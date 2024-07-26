namespace NATSInternal.Services;

/// <inheritdoc />
public class TreatmentService : ITreatmentService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPhotoService _photoService;
    private readonly IStatsService _statsService;

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
    public async Task<TreatmentListResponseDto> GetListAsync(TreatmentListRequestDto requestDto)
    {
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
                    ? query.OrderBy(t => t.Items.Sum(ti => (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                        .ThenBy(t => t.PaidDateTime)
                    : query.OrderByDescending(t => t.Items.Sum(ti => (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                        .ThenByDescending(t => t.PaidDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(t => t.PaidDateTime)
                        .ThenBy(t => t.Items.Sum(ti => (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor))
                    : query.OrderByDescending(t => t.PaidDateTime)
                        .ThenBy(t => t.Items.Sum(ti => (ti.Amount + ti.Amount * ti.VatFactor) * ti.Quantity) +
                            (t.ServiceAmount + t.ServiceAmount * t.ServiceVatFactor));
                break;
        }

        // Filter from range if specified.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(o => o.PaidDateTime >= rangeFromDateTime);
        }

        // Filter to range if specified.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value, new TimeOnly(0, 0, 0));
            query = query.Where(o => o.PaidDateTime <= rangeToDateTime);
        }

        // Filter by not being soft deleted.
        query = query.Where(o => !o.IsDeleted);

        // Initialize response dto.
        TreatmentListResponseDto responseDto = new TreatmentListResponseDto
        {
            Authorization = _authorizationService.GetTreatmentListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
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
        // Fetch the entity with the given id and ensure it exists in the database.
        return await _context.Treatments
            .Include(t => t.Customer)
            .Include(t => t.Items)
            .Include(t => t.Photos)
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new TreatmentDetailResponseDto(
                t,
                _authorizationService.GetTreatmentAuthorization(t)))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Treatment),
                nameof(id),
                id.ToString());
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

            // Check if that with the specified treatmented datetime, the new treatment will not be closed.
            if (!_statsService.VerifyResourceDateTimeToBeCreated(requestDto.PaidDateTime.Value))
            {
                DateTime minimumAllowedDateTime = _statsService
                    .GetResourceMinimumOpenedDateTime();
                string errorMessage = ErrorMessages.GreaterThanOrEqual
                    .ReplacePropertyName(DisplayNames.Treatment)
                    .ReplaceComparisonValue(minimumAllowedDateTime.ToVietnameseString());
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }

            // The PaidDateTime is valid, assign it to the new treatment.
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
        CreateItems(treatment, requestDto.Items);

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
            if (exception.InnerException is DbUpdateConcurrencyException)
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
        
        // Storing the old data for ujpdate history logging and stats adjustment.
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

            // Validate the specified PaidDateTime value from the request.
            try
            {
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

        // Update treatment properties.
        treatment.Note = requestDto.Note;
        treatment.CustomerId = requestDto.CustomerId;
        treatment.TherapistId = requestDto.TherapistId;

        // Update treatment items.
        UpdateItems(treatment, requestDto.Items);

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

        // Perform the deleting operation.
        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
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
    /// Create treatment items associated to the given treatment with the data provided
    /// in the request. This method must only be called during the treatment creating operation.
    /// </summary>
    /// <param name="treatment">
    /// The treatment which items are to be created.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the new data for the creating operation.
    /// </param>
    private void CreateItems(Treatment treatment, List<TreatmentItemRequestDto> requestDtos)
    {
        foreach (TreatmentItemRequestDto itemRequestDto in requestDtos)
        {
            TreatmentItem item = new TreatmentItem
            {
                Amount = itemRequestDto.Amount,
                VatFactor = itemRequestDto.VatFactor,
                Quantity = itemRequestDto.Quantity,
                ProductId = itemRequestDto.ProductId
            };
            treatment.Items.Add(item);
        }
    }

    /// <summary>
    /// Create treatment photos associated to the given treatment with the database provided in the request.
    /// This method must only be called during the treatment creating operation.
    /// </summary>
    /// <param name="treatment">
    /// The treatment which photos are to be created.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data for the creating operation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task CreatePhotosAsync(Treatment treatment, List<TreatmentPhotoRequestDto> requestDtos)
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
    /// Update or create treatment items associated to the given treatment with the data provided
    /// in the request. This method must only be called during the treatment updating operation.
    /// </summary>
    /// <param name="treatment">
    /// The treatment which items are to be created or updated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the new data for updating operation.
    /// </param>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    private void UpdateItems(Treatment treatment, List<TreatmentItemRequestDto> requestDtos)
    {
        for (int i = 0; i < requestDtos.Count; i++)
        {
            TreatmentItemRequestDto itemRequestDto = requestDtos[i];
            TreatmentItem item;
            if (itemRequestDto.Id.HasValue)
            {
                item = treatment.Items.SingleOrDefault(ti => ti.Id == itemRequestDto.Id.Value);

                // Throw error if the item couldn't be found.
                if (item == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.TreatmentItem);
                    throw new OperationException($"items[{i}].id", errorMessage);
                }

                // Remove item if deleted.
                if (itemRequestDto.HasBeenDeleted)
                {
                    _context.TreatmentItems.Remove(item);
                }

                // Update item properties if changed.
                if (itemRequestDto.HasBeenChanged)
                {
                    item.Amount = itemRequestDto.Amount;
                    item.VatFactor = itemRequestDto.VatFactor;
                    item.Quantity = itemRequestDto.Quantity;
                    item.ProductId = itemRequestDto.ProductId;
                }
            }
            else
            {
                item = new TreatmentItem
                {
                    Amount = itemRequestDto.Amount,
                    VatFactor = itemRequestDto.VatFactor,
                    Quantity = itemRequestDto.Quantity,
                    ProductId = itemRequestDto.ProductId
                };
                _context.TreatmentItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Update or create treatment photos associated to the given treatment with the data provided
    /// in the request. This method must only be called during the treatment updating operation.
    /// </summary>
    /// <param name="treatment">
    /// The treatment which photos are to be created or updated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the new data for updating operation.
    /// </param>
    /// <returns>
    /// A tuple containing 2 lists of photos' url strings, the first one represents the deleted photos'
    /// urls which must be deleted after the whole treatment updating operation succeeds, the second one
    /// represents the  created photos' urls which must be deleted after the whole treatment updating
    /// operation fails.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
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
                        string url = await _photoService.CreateAsync(requestDto.File, "treatments", true);
                        photo.Url = url;

                        // Mark the created photo to be deleted later if the transaction fails.
                        urlsToBeDeletedWhenFails.Add(url);
                    }
                }
            }
            else
            {
                // Create new photo if the request doesn't have id.
                string url = await _photoService.CreateAsync(requestDto.File, "treatments", true);
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
    /// Log the old and new data to update history for the specified treatment.
    /// </summary>
    /// <param name="treatment">
    /// The treatment entity which the new update history is associated.
    /// </param>
    /// <param name="oldData">
    /// An object containing the old data of the treatment before modification.
    /// </param>
    /// <param name="newData">
    /// An object containing the new data of the treatment after modification. 
    /// </param>
    /// <param name="reason">The reason of the modification.</param>
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