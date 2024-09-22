namespace NATSInternal.Services;

/// <inheritdoc cref="ITreatmentService" />
public class TreatmentService : LockableEntityService, ITreatmentService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPhotoService _photoService;
    private readonly IStatsService _statsService;
    private static MonthYearResponseDto _earliestRecordedMonthYear;

    public TreatmentService(
            DatabaseContext context,
            IAuthorizationService authorizationService,
            IPhotoService photoService,
            IStatsService statsService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _photoService = photoService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<TreatmentListResponseDto> GetListAsync(
            TreatmentListRequestDto requestDto)
    {
        // Initialize list of month and year options.
        List<MonthYearResponseDto> monthYearOptions = null;
        if (!requestDto.IgnoreMonthYear)
        {
            _earliestRecordedMonthYear ??= await _context.Treatments
                .OrderBy(s => s.PaidDateTime)
                .Select(s => new MonthYearResponseDto
                {
                    Year = s.PaidDateTime.Year,
                    Month = s.PaidDateTime.Month
                }).FirstOrDefaultAsync();
            monthYearOptions = GenerateMonthYearOptions(_earliestRecordedMonthYear);
        }

        // Initialize query.
        IQueryable<Treatment> query = _context.Treatments
            .Include(t => t.Customer)
            .Include(t => t.Items)
            .Include(t => t.Photos);

        // Sorting by direction and sorting by field filter.
        switch (requestDto.OrderByField)
        {
            case nameof(TreatmentListRequestDto.FieldOptions.Amount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(t => t.Items.Sum(ti =>
                            (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                        .ThenBy(t => t.PaidDateTime)
                    : query.OrderByDescending(t => t.Items.Sum(ti =>
                            (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                        .ThenByDescending(t => t.PaidDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(t => t.PaidDateTime)
                        .ThenBy(t => t.Items.Sum(ti =>
                            (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                    : query.OrderByDescending(t => t.PaidDateTime)
                        .ThenBy(t => t.Items.Sum(ti =>
                            (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor));
                break;
        }

        // Filter by month and year if specified.
        if (!requestDto.IgnoreMonthYear)
        {
            DateTime startDateTime = new DateTime(requestDto.Year, requestDto.Month, 1);
            DateTime endDateTime = startDateTime.AddMonths(1);
            query = query
                .Where(s => s.PaidDateTime >= startDateTime && s.PaidDateTime < endDateTime);
        }

        // Filter by user id if specified.
        if (requestDto.UserId.HasValue)
        {
            query = query.Where(t => t.CreatedUserId == requestDto.UserId);
        }

        // Filter by customer id if specified.
        if (requestDto.CustomerId.HasValue)
        {
            query = query.Where(c => c.CustomerId == requestDto.CustomerId);
        }

        // Filter by product id if specified.
        if (requestDto.ProductId.HasValue)
        {
            query = query.Where(t => t.Items.Any(oi => oi.ProductId == requestDto.ProductId));
        }

        // Filter by not being soft deleted.
        query = query.Where(o => !o.IsDeleted);

        // Initialize response dto.
        TreatmentListResponseDto responseDto = new TreatmentListResponseDto
        {
            MonthYearOptions = monthYearOptions,
            Authorization = _authorizationService.GetTreatmentListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }

        responseDto.PageCount = (int)Math.Ceiling(
            (double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(t => new TreatmentBasicResponseDto(
                t,
                _authorizationService.GetTreatmentAuthorization(t)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<TreatmentDetailResponseDto> GetDetailAsync(int id)
    {
        // Initialize query.
        IQueryable<Treatment> query = _context.Treatments
            .Include(t => t.Customer)
            .Include(t => t.Items)
            .Include(t => t.Photos);

        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessTreatmentUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(t => t.UpdateHistories);
        }

        // Fetch the entity with the given id and ensure it exists in the database.
        Treatment treatment = await query
            .AsSplitQuery()
            .SingleOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Treatment),
                nameof(id),
                id.ToString());

        return new TreatmentDetailResponseDto(
            treatment,
            _authorizationService.GetTreatmentAuthorization(treatment),
            mapUpdateHistories: shouldIncludeUpdateHistories);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(TreatmentUpsertRequestDto requestDto)
    {
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Determine PaidDateTime.
        DateTime paidDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the currentuser has permission to specify the treatmented datetime.
            if (!_authorizationService.CanSetTreatmentPaidDateTime())
            {
                throw new AuthorizationException();
            }

            paidDateTime = requestDto.PaidDateTime.Value;
        }

        // Initialize treatment entity.
        Treatment treatment = new Treatment
        {
            PaidDateTime = paidDateTime,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            Note = requestDto.Note,
            CreatedUserId = _authorizationService.GetUserId(),
            CustomerId = requestDto.CustomerId,
        };

        // Initialize treatment item entites.
        await CreateItems(treatment, requestDto.Items);

        // Initialize photos.
        if (requestDto.Photos != null)
        {
            await CreatePhotosAsync(treatment, requestDto.Photos);
        }

        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();

            // The treatment can be created successfully without any error. Add the treatment
            // to the stats.
            DateOnly treatmentedDate = DateOnly.FromDateTime(treatment.PaidDateTime);
            await _statsService.IncrementTreatmentGrossRevenueAsync(treatment.Amount, treatmentedDate);
            if (treatment.VatAmount > 0)
            {
                await _statsService.IncrementVatCollectedAmountAsync(treatment.VatAmount, treatmentedDate);
            }

            // Commit the transaction, finish the operation.
            await transaction.CommitAsync();
            return treatment.Id;
        }
        catch (DbUpdateException exception)
        {
            // Remove all the created photos.
            foreach (TreatmentPhoto photo in treatment.Photos)
            {
                _photoService.Delete(photo.Url);
            }

            // Handle the concurency exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handle the operation exception and convert to the appropriate error.
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                if (exceptionHandler.IsForeignKeyNotFound)
                {
                    string errorMessage;
                    switch (exceptionHandler.ViolatedFieldName)
                    {
                        case "customer_id":
                            errorMessage = ErrorMessages.NotFound
                                .ReplaceResourceName(DisplayNames.Customer);
                            break;
                        case "cread_user_id" or "therapist_id":
                            errorMessage = ErrorMessages.NotFound
                                .ReplaceResourceName(DisplayNames.User);
                            break;
                        default:
                            errorMessage = ErrorMessages.Undefined;
                            break;
                    }
                    throw new OperationException(errorMessage);
                }
            }
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, TreatmentUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Treatment treatment = await _context.Treatments
            .Include(t => t.CreatedUser)
            .Include(t => t.Therapist)
            .Include(t => t.Items).ThenInclude(ti => ti.Product)
            .Include(t => t.Photos)
            .SingleOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Treatment),
                nameof(id),
                id.ToString());

        // Check if the current user has permission to edit the treatment.
        if (!_authorizationService.CanEditTreatment(treatment))
        {
            throw new AuthorizationException();
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Storing the old data for update history logging and stats adjustment.
        long oldAmount = treatment.Amount;
        long oldVatAmount = treatment.VatAmount;
        DateOnly oldPaidDate = DateOnly.FromDateTime(treatment.PaidDateTime);
        TreatmentUpdateHistoryDataDto oldData = new TreatmentUpdateHistoryDataDto(treatment);

        // Handle the new treatmented datetime when the request specifies it.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify a new treatmented datetime.
            if (!_authorizationService.CanSetTreatmentPaidDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the treatment's PaidDateTime to be modified when the treatment is locked.
            if (treatment.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Treatment)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(
                    nameof(requestDto.PaidDateTime),
                    errorMessage);
            }

            // Assign the new PaidDateTime value only if it's different from the old one.
            if (requestDto.PaidDateTime.Value != treatment.PaidDateTime)
            {
                // Validate and assign the specified PaidDateTime value from the request.
                try
                {
                    _statsService.ValidateStatsDateTime(treatment, requestDto.PaidDateTime.Value);
                    treatment.PaidDateTime = requestDto.PaidDateTime.Value;
                }
                catch (ArgumentException exception)
                {
                    string errorMessage = exception.Message
                        .ReplacePropertyName(DisplayNames.PaidDateTime);
                    throw new OperationException(
                        nameof(requestDto.PaidDateTime),
                        errorMessage);
                }
            }
        }

        // Update treatment properties.
        treatment.Note = requestDto.Note;
        treatment.TherapistId = requestDto.TherapistId;

        // Update treatment items.
        await UpdateItems(treatment, requestDto.Items);

        // Update photos.
        List<string> urlsToBeDeletedWhenSucceeds = new List<string>();
        List<string> urlsToBeDeletedWhenFails = new List<string>();
        if (requestDto.Photos != null)
        {
            (List<string>, List<string>) photoUpdateResults;
            photoUpdateResults = await UpdatePhotoAsync(treatment, requestDto.Photos);
            urlsToBeDeletedWhenSucceeds.AddRange(photoUpdateResults.Item1);
            urlsToBeDeletedWhenFails.AddRange(photoUpdateResults.Item2);
        }
        
        // Store new data for update history logging.
        TreatmentUpdateHistoryDataDto newData = new TreatmentUpdateHistoryDataDto(treatment);
        
        // Log update history.
        LogUpdateHistory(treatment, oldData, newData, requestDto.UpdateReason);

        // Save changes and handle the errors.
        try
        {
            // Save all modifications.
            await _context.SaveChangesAsync();

            // The treatment can be updated successfully without any error.
            // Adjust the stats for items' amount and vat collected amount.
            // Revert the old stats.
            await _statsService.IncrementTreatmentGrossRevenueAsync(-oldAmount, oldPaidDate);
            await _statsService.IncrementVatCollectedAmountAsync(-oldVatAmount, oldPaidDate);
            
            // Create new stats.
            DateOnly newPaidDate = DateOnly.FromDateTime(treatment.PaidDateTime);
            await _statsService.IncrementRetailGrossRevenueAsync(treatment.Amount, newPaidDate);
            await _statsService.IncrementVatCollectedAmountAsync(treatment.VatAmount, newPaidDate);

            // Delete all old photos which have been replaced by new ones.
            foreach (string url in urlsToBeDeletedWhenSucceeds)
            {
                _photoService.Delete(url);
            }
            
            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        {
            // Undo all the created photos.
            foreach (string url in urlsToBeDeletedWhenFails)
            {
                _photoService.Delete(url);
            }

            // Handle concurrency exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handling the exception in the foreseen cases.
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                if (exceptionHandler.IsForeignKeyNotFound)
                {
                    switch (exceptionHandler.ViolatedFieldName)
                    {
                        case "customer_id":
                            throw new ResourceNotFoundException(
                                nameof(Customer),
                                nameof(requestDto.CustomerId),
                                requestDto.CustomerId.ToString());
                        case "therapist_id":
                            throw new ResourceNotFoundException(
                                nameof(User),
                                nameof(requestDto.TherapistId),
                                requestDto.TherapistId.ToString());
                        case "created_user_id":
                            throw new ConcurrencyException();
                    }
                }
            }
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure it exists.
        Treatment treatment = await _context.Treatments
            .Include(t => t.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Photos)
            .Include(t => t.Photos)
            .SingleOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Treatment),
                nameof(id),
                id.ToString());

        // Check if the current user has permission to delete the order.
        if (!_authorizationService.CanDeleteTreatment())
        {
            throw new AuthorizationException();
        }

        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Delete all the items associated to this treatment.
        DeleteItemsAsync(treatment);

        // Delete the treatment entity.
        _context.Treatments.Remove(treatment);

        // Perform the deleting operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The treatment can be deleted successfully without any error.
            // Revert the stats associated to the treatment.
            DateOnly paidDate = DateOnly.FromDateTime(treatment.PaidDateTime);
            await _statsService.IncrementTreatmentGrossRevenueAsync(-treatment.Amount, paidDate);
            await _statsService.IncrementVatCollectedAmountAsync(-treatment.VatAmount, paidDate);

            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();

            // Deleted all the created photo files for the supply.
            foreach (string url in treatment.Photos.Select(p => p.Url).ToList())
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        {
            // Handle the concurrency exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handle operation exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                if (exceptionHandler.IsDeleteOrUpdateRestricted)
                {
                    // Soft delete when there are any other related entities which
                    // are restricted to be deleted.
                    treatment.IsDeleted = true;

                    // Save changes.
                    await _context.SaveChangesAsync();

                    // Order has been deleted successfully, adjust the stats.
                    DateOnly orderedDate = DateOnly.FromDateTime(treatment.PaidDateTime);
                    await _statsService.IncrementRetailGrossRevenueAsync(
                        treatment.Amount,
                        orderedDate);
                    await _statsService.IncrementVatCollectedAmountAsync(
                        treatment.VatAmount,
                        orderedDate);

                    // Commit the transaction and finishing the operations.
                    await transaction.CommitAsync();
                }
            }
            throw;
        }
    }

    /// <summary>
    /// Creates treatment items associated to the given treatment with the data provided in the
    /// request.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the treatment creating operation.
    /// </remarks>
    /// <param name="treatment">
    /// An instance of the <see cref="Treatment"/> entity class, representing the treatment to
    /// which the creating items are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is the instances of the
    /// <see cref="TreatmentItemRequestDto"/> class, containing the new data for the creating
    /// operation.
    /// </param>
    private async Task CreateItems(
            Treatment treatment,
            List<TreatmentItemRequestDto> requestDtos)
    {
        // Fetch a list of products which ids are specified in the request.
        List<int> requestedProductIds = requestDtos.Select(i => i.ProductId).ToList();
        List<Product> products = await _context.Products
            .Where(p => requestedProductIds.Contains(p.Id))
            .ToListAsync();
        
        for (int i = 0; i < requestDtos.Count; i++)
        {
            TreatmentItemRequestDto itemRequestDto = requestDtos[i];
            // Get the product with the specified id from pre-fetched list.
            Product product = products.SingleOrDefault(p => p.Id == itemRequestDto.Id);

            // Ensure the product exists.
            if (product == null)
            {
                string errorMessage = ErrorMessages.NotFoundByProperty
                    .ReplaceResourceName(DisplayNames.Product)
                    .ReplacePropertyName(DisplayNames.Id)
                    .ReplaceAttemptedValue(itemRequestDto.ProductId.ToString());
                throw new OperationException($"items[{i}].productId", errorMessage);
            }

            // Validate that with the specified quantity value.
            if (product.StockingQuantity < itemRequestDto.Quantity)
            {
                string errorMessage = ErrorMessages.NegativeProductStockingQuantity;
                throw new OperationException($"items[{i}].quantity", errorMessage);
            }

            // Initialize entity.
            TreatmentItem item = new TreatmentItem
            {
                Amount = itemRequestDto.Amount,
                VatFactor = itemRequestDto.VatFactor,
                Quantity = itemRequestDto.Quantity,
                Product = product
            };

            // Adjust the stocking quantity of the associated product.
            product.StockingQuantity -= item.Quantity;

            // Add item.
            treatment.Items.Add(item);
        }
    }

    /// <summary>
    /// Creates treatment photos associated to the given treatment with the data provided in
    /// the request.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the treatment creating operation.
    /// </remarks>
    /// <param name="treatment">
    /// An instance of the <see cref="Treatment"/> entity class, representing the treatment
    /// with which the creating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is the instances of the
    /// <see cref="TreatmentPhotoRequestDto"/> class, containing the data for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    private async Task CreatePhotosAsync(
            Treatment treatment,
            List<TreatmentPhotoRequestDto> requestDtos)
    {
        foreach (TreatmentPhotoRequestDto photoRequestDto in requestDtos)
        {
            string url = await _photoService.CreateAsync(photoRequestDto.File, "treatments", false);
            TreatmentPhoto photo = new TreatmentPhoto
            {
                Url = url
            };
            treatment.Photos.Add(photo);
        }
    }

    /// <summary>
    /// Updates or creates treatment items associated to the given treatment with the data
    /// provided in the request.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the treatment updating operation.
    /// </remarks>
    /// <param name="treatment">
    /// An instance of the <see cref="Treatment"/> entity class, representing the treatment
    /// with which the updating and creating items are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is the instances of the
    /// <see cref="TreatmentItemRequestDto"/> class, containing the new data for the updating
    /// operation.
    /// </param>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:
    /// - When the item with the specified id doesn't exist or has already been deleted.
    /// - When the specified product with which the item is associated doesn't exist or has
    /// already been deleted.
    /// </exception>
    private async Task UpdateItems(
            Treatment treatment,
            List<TreatmentItemRequestDto> requestDtos)
    {
        // Pre-fetch a list of products for new items which ids are specfied in the request.
        List<int> productIdsForNewItems = requestDtos
            .Where(i => !i.Id.HasValue)
            .Select(i => i.ProductId)
            .ToList();
        List<Product> productsForNewItems = await _context.Products
            .Where(p => productIdsForNewItems.Contains(p.Id))
            .ToListAsync();

        for (int i = 0; i < requestDtos.Count; i++)
        {
            TreatmentItemRequestDto itemRequestDto = requestDtos[i];
            TreatmentItem item;
            if (itemRequestDto.Id.HasValue)
            {
                item = treatment.Items.SingleOrDefault(ti => ti.Id == itemRequestDto.Id.Value);

                // Ensure the item exists.
                if (item == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.TreatmentItem);
                    throw new OperationException($"items[{i}].id", errorMessage);
                }

                // Revert the added stocking quantity of the product associated to the item.
                item.Product.StockingQuantity -= item.Quantity;

                // Remove item if deleted.
                if (itemRequestDto.HasBeenDeleted)
                {
                    _context.TreatmentItems.Remove(item);
                }

                // Update item properties if changed.
                else if (itemRequestDto.HasBeenChanged)
                {
                    item.Amount = itemRequestDto.Amount;
                    item.VatFactor = itemRequestDto.VatFactor;
                    item.Quantity = itemRequestDto.Quantity;
                }
            }
            else
            {
                // Get the product entity from the pre-fetched products list.
                Product product = productsForNewItems
                    .Single(p => p.Id == itemRequestDto.ProductId);
                
                // Ensure the product exists.
                if (product == null)
                {
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplaceResourceName(DisplayNames.Product)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(itemRequestDto.ProductId.ToString());
                    throw new OperationException($"items[{i}].productId", errorMessage);
                }

                item = new TreatmentItem
                {
                    Amount = itemRequestDto.Amount,
                    VatFactor = itemRequestDto.VatFactor,
                    Quantity = itemRequestDto.Quantity,
                    Product = product
                };
                _context.TreatmentItems.Add(item);
            }

            // Validate the new quantity value.
            if (item.Product.StockingQuantity - item.Quantity < 0)
            {
                string errorMessage = ErrorMessages.NegativeProductStockingQuantity;
                throw new OperationException($"items[{i}].stockingQuantity", errorMessage);
            }

            // Add the quantity to the product's stocking quantity.
            item.Product.StockingQuantity += item.Quantity;
        }
    }

    /// <summary>
    /// Updates or creates treatment photos associated to the given treatment with the data
    /// provided in the request.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the treatment updating operation.
    /// </remarks>
    /// <param name="treatment">
    /// An instance of the <see cref="Treatment"/> entity class, representing the treatment
    /// with which the updating and creating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is the instances of the
    /// <see cref="TreatmentPhotoRequestDto"/> class, containing the new data for updating
    /// operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is a
    /// <see cref="Tuple"/> of two <see cref="List{T}"/> where <c>T</c> is
    /// <see cref="string"/>, the first one represents the deleted photos' urls which must be
    /// deleted after the whole treatment updating operation succeeds, the second one
    /// represents the  created photos' urls which must be deleted after the whole treatment
    /// updating operation fails.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws when the photo with the specified id doesn't exist or has already been deleted.
    /// </exception>
    public async Task<(List<string>, List<string>)> UpdatePhotoAsync(
            Treatment treatment,
            List<TreatmentPhotoRequestDto> requestDtos)
    {
        List<string> urlsToBeDeletedWhenSucceeds = new List<string>();
        List<string> urlsToBeDeletedWhenFails = new List<string>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            TreatmentPhotoRequestDto requestDto = requestDtos[i];
            TreatmentPhoto photo;
            if (requestDto.Id.HasValue)
            {
                // Fetch the photo entity with the given id from the request.
                photo = treatment.Photos.SingleOrDefault(op => op.Id == requestDto.Id);

                // Ensure the photo entity exists.
                if (photo == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplacePropertyName(DisplayNames.Photo)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(requestDto.Id.ToString());
                    throw new OperationException($"photos[{i}]", errorMessage);
                }

                // Perform the modification when the photo is marked to have been changed.
                if (requestDto.HasBeenChanged)
                {
                    // Mark the current url to be deleted later when the transaction succeeds.
                    urlsToBeDeletedWhenSucceeds.Add(photo.Url);

                    // Create new photo if the request contains new data for a new one.
                    if (requestDto.File != null)
                    {
                        string url = await _photoService
                            .CreateAsync(requestDto.File, "treatments", true);
                        photo.Url = url;

                        // Mark the created photo to be deleted later if the transaction fails.
                        urlsToBeDeletedWhenFails.Add(url);
                    }
                }
            }
            else
            {
                // Create new photo if the request doesn't have id.
                string url = await _photoService
                    .CreateAsync(requestDto.File, "treatments", true);
                photo = new TreatmentPhoto
                {
                    Url = url
                };
                treatment.Photos.Add(photo);

                // Mark the created photo to be deleted later if the transaction fails.
                urlsToBeDeletedWhenFails.Add(url);
            }
        }

        return (urlsToBeDeletedWhenSucceeds, urlsToBeDeletedWhenFails);
    }
    
    /// <summary>
    /// Deletes all the items associated to the specified treatment, revert the stocking
    /// quantity of the products associated to each item.
    /// </summary>
    /// <param name="treatment">
    /// An instance of the <c>Treatment</c> entity class with which the deleting items are
    /// associated.
    /// </param>
    private void DeleteItemsAsync(Treatment treatment)
    {
        foreach (TreatmentItem item in treatment.Items)
        {
            item.Product.StockingQuantity += item.Quantity;
            _context.TreatmentItems.Remove(item);
        }
    }

    /// <summary>
    /// Logs the old and new data to update history for the specified treatment.
    /// </summary>
    /// <param name="treatment">
    /// An instance of the <see cref="Treatment"/> entity class, representing the treatment to
    /// be logged.
    /// </param>
    /// <param name="oldData">
    /// An instance of the <see cref="TreatmentUpdateHistoryDataDto"/> class, containing the
    /// old data of the treatment before the modification.
    /// </param>
    /// <param name="newData">
    /// An instance of the <see cref="TreatmentUpdateHistoryDataDto"/> class, containing the
    /// new data of the treatment before the modification.
    /// </param>
    /// <param name="reason">
    /// A <see cref="string"/> value representing the reason of the modification.
    /// </param>
    private void LogUpdateHistory(
            Treatment treatment,
            TreatmentUpdateHistoryDataDto oldData,
            TreatmentUpdateHistoryDataDto newData,
            string reason)
    {
        TreatmentUpdateHistory updateHistory = new TreatmentUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId()
        };
        
        treatment.UpdateHistories ??= new List<TreatmentUpdateHistory>();
        treatment.UpdateHistories.Add(updateHistory);
    }
}