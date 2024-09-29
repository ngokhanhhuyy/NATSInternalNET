namespace NATSInternal.Services;

/// <inheritdoc cref="ISupplyService" />
public class SupplyService : LockableEntityService, ISupplyService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
    private static MonthYearResponseDto _earliestRecordedMonthYear;

    public SupplyService(
            DatabaseContext context,
            IPhotoService photoservice,
            IAuthorizationService authorizationService,
            IStatsService statsService)
    {
        _context = context;
        _photoService = photoservice;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<SupplyListResponseDto> GetListAsync(SupplyListRequestDto requestDto)
    {
        // Initialize list of month and year options.
        List<MonthYearResponseDto> monthYearOptions = null;
        if (!requestDto.IgnoreMonthYear)
        {
            _earliestRecordedMonthYear ??= await _context.Supplies
                .OrderBy(s => s.PaidDateTime)
                .Select(s => new MonthYearResponseDto
                {
                    Year = s.PaidDateTime.Year,
                    Month = s.PaidDateTime.Month
                }).FirstOrDefaultAsync();

            monthYearOptions = GenerateMonthYearOptions(_earliestRecordedMonthYear);
        }

        // Query initialization.
        IQueryable<Supply> query = _context.Supplies
            .Include(s => s.CreatedUser).ThenInclude(u => u.Roles)
            .Include(s => s.Items)
            .Include(s => s.Photos);

        // Sorting directing and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(SupplyListRequestDto.FieldOptions.TotalAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.TotalAmount)
                        .ThenBy(s => s.PaidDateTime)
                    : query.OrderByDescending(s => s.Items.Sum(i => i.Amount))
                        .ThenByDescending(s => s.PaidDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.PaidDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.PaidDateTime)
                        .ThenBy(s => s.Items.Sum(i => i.Amount))
                    : query.OrderByDescending(s => s.PaidDateTime)
                        .ThenByDescending(s => s.Items.Sum(i => i.Amount));
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ItemAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ItemAmount)
                        .ThenBy(s => s.PaidDateTime)
                    : query.OrderByDescending(s => s.ItemAmount)
                        .ThenByDescending(s => s.PaidDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ShipmentFee):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ShipmentFee)
                        .ThenBy(s => s.PaidDateTime)
                    : query.OrderByDescending(s => s.ShipmentFee)
                        .ThenByDescending(s => s.PaidDateTime);
                break;
        }

        // Filter by month and year if specified.
        if (!requestDto.IgnoreMonthYear)
        {
            DateTime startDateTime;
            startDateTime = new DateTime(requestDto.Year, requestDto.Month, 1);
            DateTime endDateTime = startDateTime.AddMonths(1);
            query = query
                .Where(s => s.PaidDateTime >= startDateTime && s.PaidDateTime < endDateTime);
        }

        // Filter by user id if specified.
        if (requestDto.CreatedUserId.HasValue)
        {
            query = query.Where(s => s.CreatedUserId == requestDto.CreatedUserId.Value);
        }

        // Filter by product id if specified.
        if (requestDto.ProductId.HasValue)
        {
            query = query.Where(s => s.Items.Any(si => si.ProductId == requestDto.ProductId));
        }

        // Initialize response dto.
        SupplyListResponseDto responseDto = new SupplyListResponseDto
        {
            MonthYearOptions = monthYearOptions,
            Authorization = _authorizationService.GetSupplyListAuthorization()
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
            .Select(s => new SupplyBasicResponseDto(s))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSingleQuery()
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<SupplyDetailResponseDto> GetDetailAsync(int id)
    {
        // Initialize query.
        IQueryable<Supply> query = _context.Supplies
            .Include(s => s.Items).ThenInclude(si => si.Product)
            .Include(s => s.Photos)
            .Include(s => s.CreatedUser).ThenInclude(u => u.Roles);

        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessSupplyUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(s => s.UpdateHistories);
        }

        // Fetch the entity with the given id and ensure it exists in the database.
        Supply supply = await query
            .AsSplitQuery()
            .SingleOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Supply),
                nameof(id),
                id.ToString());

        return new SupplyDetailResponseDto(
            supply,
            _authorizationService.GetSupplyAuthorization(supply),
            mapUpdateHistories: shouldIncludeUpdateHistories);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(SupplyUpsertRequestDto requestDto)
    {
        // Use a transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Determine the PaidDateTime.
        DateTime paidDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the PaidDateTime.
            if (!_authorizationService.CanSetSupplyPaidDateTime())
            {
                throw new AuthorizationException();
            }

            paidDateTime = requestDto.PaidDateTime.Value;
        }

        // Initialize entity.
        Supply supply = new Supply
        {
            PaidDateTime = paidDateTime,
            ShipmentFee = requestDto.ShipmentFee,
            Note = requestDto.Note,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            CreatedUserId = _authorizationService.GetUserId(),
            Items = new List<SupplyItem>(),
            Photos = new List<SupplyPhoto>()
        };
        _context.Supplies.Add(supply);

        // Initialize items
        await CreateItemsAsync(supply, requestDto.Items);

        // Initialize photos
        if (requestDto.Photos != null)
        {
            await CreatePhotosAsync(supply, requestDto.Photos);
        }

        // Save changes.
        try
        {
            await _context.SaveChangesAsync();

            // The supply can be saved successfully without any error.
            // Add new stats for the created supply.
            await _statsService.IncrementSupplyCostAsync(supply.ItemAmount);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee);

            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();

            return supply.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            await transaction.RollbackAsync();
            // Delete all created photos.
            foreach (SupplyPhoto supplyPhoto in supply.Photos)
            {
                _photoService.Delete(supplyPhoto.Url);
            }

            HandleCreateOrUpdateException(sqlException);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, SupplyUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Supply supply = await _context.Supplies
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .Include(s => s.Photos)
            .Where(s => s.Id == id)
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Supply),
                nameof(id),
                id.ToString());

        // Ensure the user has permission to edit the supply.
        if (!_authorizationService.CanEditSupply(supply))
        {
            throw new AuthorizationException();
        }

        // Use transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Storing the old data for update history logging and stats adjustments.
        SupplyUpdateHistoryDataDto oldData = new SupplyUpdateHistoryDataDto(supply);
        long oldItemAmount = supply.ItemAmount;
        long oldShipmentFee = supply.ShipmentFee;
        DateOnly oldPaidDate = DateOnly.FromDateTime(supply.PaidDateTime);

        // Determining PaidDateTime.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Restrict the PaidDateTime to be modified after being locked.
            if (supply.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Supply)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }

            // Validate PaidDateTime.
            try
            {
                supply.PaidDateTime = requestDto.PaidDateTime.Value;
            }
            catch (ArgumentException exception)
            {
                string errorMessage = exception.Message
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }

            supply.PaidDateTime = requestDto.PaidDateTime.Value;
        }

        // Update supply properties.
        supply.ShipmentFee = requestDto.ShipmentFee;
        supply.Note = requestDto.Note;

        // Update supply items.
        await UpdateItemsAsync(supply, requestDto.Items);

        // Update photos.
        List<string> urlsToBeDeletedWhenSucceed = new List<string>();
        List<string> urlsToBeDeletedWhenFails = new List<string>();
        if (requestDto.Photos != null)
        {
            (List<string>, List<string>) photoUpdateResults;
            photoUpdateResults = await UpdatePhotosAsync(supply, requestDto.Photos);
            urlsToBeDeletedWhenSucceed.AddRange(photoUpdateResults.Item1);
            urlsToBeDeletedWhenFails.AddRange(photoUpdateResults.Item2);
        }

        // Storing new data for update history logging.
        SupplyUpdateHistoryDataDto newData = new SupplyUpdateHistoryDataDto(supply);

        // Log update history.
        LogUpdateHistory(supply, oldData, newData, requestDto.UpdateReason);

        // Perform the updating operation.
        try
        {
            await _context.SaveChangesAsync();

            // The supply can be saved without any error, adjust the stats.
            // Revert the old stats.
            await _statsService.IncrementSupplyCostAsync(-oldItemAmount, oldPaidDate);
            await _statsService.IncrementShipmentCostAsync(-oldShipmentFee, oldPaidDate);

            // Add new stats.
            DateOnly newPaidDate = DateOnly.FromDateTime(supply.PaidDateTime);
            await _statsService.IncrementShipmentCostAsync(supply.ItemAmount, newPaidDate);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee, newPaidDate);

            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();
            foreach (string url in urlsToBeDeletedWhenSucceed)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            if (exception.InnerException is MySqlException sqlException)
            {
                await transaction.RollbackAsync();
                foreach (string url in urlsToBeDeletedWhenFails)
                {
                    _photoService.Delete(url);
                }
                HandleCreateOrUpdateException(sqlException);
            }
            
            throw;
        }

    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity with given id from the database and ensure it exists.
        Supply supply = await _context.Supplies
            .Include(s => s.Items).ThenInclude(si => si.Product)
            .Include(s => s.Photos)
            .SingleOrDefaultAsync(s => s.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(Supply),
                nameof(id),
                id.ToString());

        if (!_authorizationService.CanDeleteSupply(supply))
        {
            throw new AuthorizationException();
        }

        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Remove all the items.
        DeleteItems(supply);

        // Delete the supply entity.
        _context.Supplies.Remove(supply);

        try
        {
            await _context.SaveChangesAsync();

            // The supply can be deleted successfully without any error.
            // Revert the stats associated to the supply.
            DateOnly paidDate = DateOnly.FromDateTime(supply.PaidDateTime);
            await _statsService.IncrementSupplyCostAsync(-supply.ItemAmount, paidDate);
            await _statsService.IncrementSupplyCostAsync(-supply.ShipmentFee, paidDate);

            // Commit transaction and finish the operation.
            await transaction.CommitAsync();

            // Delete all supply photos after transaction succeeded.
            if (supply.Photos != null)
            {
                foreach (string url in supply.Photos.Select(p => p.Url))
                {
                    _photoService.Delete(url);
                }
            }
        }
        catch (DbUpdateException exception)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            if (exception.InnerException is MySqlException sqlException)
            {
               HandleDeleteExeption(sqlException);
            }
            
            throw;
        }

    }

    /// <summary>
    /// Convert all the exceptions those are thrown by the database during the creating
    /// or updating operation into the appropriate execptions.
    /// </summary>
    /// <param name="exception">The exeception thrown by the database.</param>
    /// <exception cref="OperationException"></exception>
    private static void HandleCreateOrUpdateException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            if (exceptionHandler.ViolatedFieldName == "product_id")
            {
                string errorMessage = ErrorMessages.NotFound
                    .ReplaceResourceName(DisplayNames.Product);
                throw new OperationException($"items.productId", errorMessage);
            }
        }
    }

    /// <summary>
    /// Convert all the exceptions those are thrown by the database during
    /// the deleting operation into the appropriate exceptions.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    /// <exception cref="OperationException"></exception>
    private static void HandleDeleteExeption(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsDeleteOrUpdateRestricted)
        {
            string errorMessage = ErrorMessages.DeleteRestricted
                .ReplaceResourceName(DisplayNames.Supply);
            throw new OperationException(errorMessage);
        }
    }

    /// <summary>
    /// Create and add items for the specified <c>Supply</c> with the data provided
    /// in the request.
    /// </summary>
    /// <param name="supply">
    /// The supply entity to which the items are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data for the new items.
    /// </param>
    /// <returns>
    /// A <c>Task</c> object representing the asynchronous operation.
    /// </returns>
    private async Task CreateItemsAsync(
            Supply supply,
            List<SupplyItemRequestDto> requestDtos)
    {
        // Pre-fetch the list of products with the specified ids in the request.
        List<int> requestedProductIds = requestDtos.Select(sp => sp.ProductId).ToList();
        List<Product> products = await _context.Products
            .Where(p => requestedProductIds.Contains(p.Id))
            .ToListAsync();

        for (int i = 0; i < requestDtos.Count; i++)
        {
            SupplyItemRequestDto itemRequestDto = requestDtos[i];

            // Fetch the product entity with the specified id in the request.
            Product product = products
                .SingleOrDefault(p => p.Id == itemRequestDto.ProductId);

            // Ensure the product exists.
            if (product == null)
            {
                string errorMessage = ErrorMessages.NotFoundByProperty
                    .ReplacePropertyName(DisplayNames.Id)
                    .ReplaceResourceName(DisplayNames.Product)
                    .ReplaceAttemptedValue(itemRequestDto.ProductId.ToString());
                throw new OperationException($"items[{i}].id", errorMessage);
            }

            // Initialize item entity.
            SupplyItem supplyItem = new()
            {
                Amount = itemRequestDto.Amount,
                SuppliedQuantity = itemRequestDto.SuppliedQuantity,
                ProductId = product.Id
            };

            // Increment product stocking quantity.
            product.StockingQuantity += itemRequestDto.SuppliedQuantity;

            // Add created item to the supply.
            supply.Items.Add(supplyItem);
        }
    }

    /// <summary>
    /// Update the specified supply's items with the data provided in the request.
    /// </summary>
    /// <param name="supply">
    /// The supply associated to the items to be updated.
    /// </param>
    /// <param name="requestDtos">
    /// An object containing data for the items to be updated.
    /// </param>
    private async Task UpdateItemsAsync(
            Supply supply,
            List<SupplyItemRequestDto> requestDtos)
    {
        // Fetch a list of products for the items which are indicated to be created.
        List<int> productIdsForNewItems;
        productIdsForNewItems = requestDtos
            .Where(i => !i.Id.HasValue)
            .Select(i => i.ProductId)
            .ToList();
        List<Product> productsForNewItems = await _context.Products
            .Where(p => productIdsForNewItems.Contains(p.Id))
            .ToListAsync();

        supply.Items ??= new List<SupplyItem>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            SupplyItemRequestDto itemRequestDto = requestDtos[i];
            if (itemRequestDto.HasBeenChanged)
            {
                SupplyItem item;

                // Initialize a new entity when the request doesn't have id.
                if (itemRequestDto.Id.HasValue)
                {
                    // Get the entity by the given id and ensure it exists.
                    item = supply.Items.SingleOrDefault(si => si.Id == itemRequestDto.Id);
                    if (item == null)
                    {
                        string errorMessage = ErrorMessages.NotFoundByProperty
                            .ReplaceResourceName(DisplayNames.SupplyItem)
                            .ReplacePropertyName(DisplayNames.Id)
                            .ReplaceAttemptedValue(itemRequestDto.Id.ToString());
                        throw new OperationException($"items[{i}].id", errorMessage);
                    }

                    // Delete the entity if specified.
                    if (itemRequestDto.HasBeenDeleted)
                    {
                        supply.Items.Remove(item);
                        continue;
                    }

                    item.Amount = itemRequestDto.Amount;
                    item.Product.StockingQuantity -= item.SuppliedQuantity;
                    item.SuppliedQuantity = itemRequestDto.SuppliedQuantity;
                }
                else
                {
                    // Get the product entity from the pre-fetched list.
                    Product product = productsForNewItems
                        .SingleOrDefault(p => p.Id == itemRequestDto.ProductId);

                    // Ensure the product exists in the database.
                    if (product == null)
                    {
                        string errorMessage = ErrorMessages.NotFoundByProperty
                            .ReplaceResourceName(DisplayNames.Product)
                            .ReplacePropertyName(DisplayNames.Id)
                            .ReplaceAttemptedValue(itemRequestDto.ProductId.ToString());
                        throw new OperationException($"item[{i}].productId", errorMessage);
                    }

                    // Initialize new supply item.
                    item = new SupplyItem
                    {
                        Amount = itemRequestDto.Amount,
                        SuppliedQuantity = itemRequestDto.SuppliedQuantity,
                        ProductId = itemRequestDto.ProductId
                    };
                    supply.Items.Add(item);
                }

                // Adjust product stocking quantity.
                item.Product.StockingQuantity += itemRequestDto.SuppliedQuantity;
            }
        }
    }

    /// <summary>
    /// Delete all the items associated to the supply and revert the stocking quantity
    /// of the products associated to each item.
    /// </summary>
    /// <param name="supply">The supply of which the items are to be deleted.</param>
    private void DeleteItems(Supply supply)
    {
        foreach (SupplyItem item in supply.Items)
        {
            // Revert the stocking quantity of the product associated to the item.
            item.Product.StockingQuantity -= item.SuppliedQuantity;

            // Remove the item.
            _context.SupplyItems.Remove(item);
        }
    }

    /// <summary>
    /// Create photos which are associated to the specified supply with the data
    /// provided in the request.
    /// </summary>
    /// <param name="supply">
    /// The <c>Supply</c> entity to which the photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data for the new photos.
    /// </param>
    /// <returns>
    /// A <c>Task</c> object reprensenting the asynchronous operation.
    /// </returns>
    private async Task CreatePhotosAsync(
            Supply supply,
            List<SupplyPhotoRequestDto> requestDtos)
    {
        for (int i = 0; i < requestDtos.Count; i++)
        {
            SupplyPhotoRequestDto photoRequestDto = requestDtos[i];
            string url = await _photoService
                .CreateAsync(photoRequestDto.File, "supplies", false);
            SupplyPhoto supplyPhoto = new()
            {
                Url = url
            };

            supply.Photos.Add(supplyPhoto);
        }
    }

    /// <summary>
    /// Update the specified supply's photos with the data provided in the request.
    /// </summary>
    /// <param name="supply">
    /// The supply to which the updating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// An object containing the data for the photos to be updated.
    /// </param>
    /// <returns>
    /// A <c>Tuple</c> containing 2 lists of strings. The first one contains the urls
    /// of the photos which must be deleted when the update operation succeeded. The
    /// other one contains the urls of the photos which must be deleted when the
    /// updating operation failed.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when the photo with the given id which is associated to the specified
    /// supply in the request cannot be found.
    /// </exception>
    private async Task<(List<string>, List<string>)> UpdatePhotosAsync(
            Supply supply,
            List<SupplyPhotoRequestDto> requestDtos)
    {
        supply.Photos ??= new List<SupplyPhoto>();
        List<string> urlsToBeDeletedWhenSucceeded = new List<string>();
        List<string> urlsToBeDeletedWhenFailed = new List<string>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            SupplyPhotoRequestDto photoRequestDto = requestDtos[i];
            if (photoRequestDto.HasBeenChanged)
            {
                // Fetch the photo entity and ensure it exists.
                SupplyPhoto supplyPhoto = supply.Photos
                    .SingleOrDefault(p => p.Id == photoRequestDto.Id);
                if (supplyPhoto == null)
                {
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplaceResourceName(DisplayNames.SupplyPhoto)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(photoRequestDto.Id.ToString());
                    throw new OperationException($"photos[{i}].id", errorMessage);
                }

                // Add to list to be deleted later if the transaction succeeds.
                urlsToBeDeletedWhenSucceeded.Add(supplyPhoto.Url);
                supply.Photos.Remove(supplyPhoto);

                if (photoRequestDto.HasBeenChanged)
                {
                    string url = await _photoService
                        .CreateAsync(photoRequestDto.File, "supplies", false);
                    // Add to list to be deleted later if the transaction fails.
                    urlsToBeDeletedWhenFailed.Add(url);
                    supplyPhoto.Url = url;
                }
            }
        }

        return (urlsToBeDeletedWhenSucceeded, urlsToBeDeletedWhenFailed);
    }

    /// <summary>
    /// Log the old and new data to update history for the specified supply.
    /// </summary>
    /// <param name="supply">
    /// The supply entity which the new update history is associated.
    /// </param>
    /// <param name="oldData">
    /// An object containing the old data of the supply before modification.
    /// </param>
    /// <param name="newData">
    /// An object containing the new data of the supply after modification. 
    /// </param>
    /// <param name="reason">The reason of the modification.</param>
    private void LogUpdateHistory(
            Supply supply,
            SupplyUpdateHistoryDataDto oldData,
            SupplyUpdateHistoryDataDto newData,
            string reason)
    {
        SupplyUpdateHistory updateHistory = new SupplyUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId()
        };

        supply.UpdateHistories ??= new List<SupplyUpdateHistory>();
        supply.UpdateHistories.Add(updateHistory);
    }
}
