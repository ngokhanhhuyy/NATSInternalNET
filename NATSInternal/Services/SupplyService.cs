namespace NATSInternal.Services;

/// <inheritdoc />
public class SupplyService : ISupplyService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;

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
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.Items.Sum(i => i.Amount))
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.SuppliedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.SuppliedDateTime)
                        .ThenBy(s => s.Items.Sum(i => i.Amount))
                    : query.OrderByDescending(s => s.SuppliedDateTime)
                        .ThenByDescending(s => s.Items.Sum(i => i.Amount));
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ItemAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ItemAmount)
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.ItemAmount)
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ShipmentFee):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ShipmentFee)
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.ShipmentFee)
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
        }

        // Filter from range if specified.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.SuppliedDateTime >= rangeFromDateTime);
        }

        // Filter to range if specified.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.SuppliedDateTime <= rangeToDateTime);
        }

        // Filter by user id if specified.
        if (requestDto.UserId.HasValue)
        {
            query = query.Where(s => s.CreatedUserId == requestDto.UserId.Value);
        }

        // Initialize response dto.
        SupplyListResponseDto responseDto = new SupplyListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(s => new SupplyBasicResponseDto(s))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<SupplyDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Supplies
            .Include(s => s.Items).ThenInclude(si => si.Product)
            .Include(s => s.Photos)
            .Include(s => s.CreatedUser).ThenInclude(u => u.Roles)
            .Include(s => s.UpdateHistories)
            .Where(s => s.Id == id)
            .Select(s => new SupplyDetailResponseDto(s, _authorizationService.GetSupplyAuthorization(s)))
            .AsSplitQuery()
            .SingleOrDefaultAsync()
        ?? throw new ResourceNotFoundException(
            nameof(Supply),
            nameof(id),
            id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(SupplyUpsertRequestDto requestDto)
    {
        // Use a transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Fetch a list of products given by id in the request, ensure all of the products exists.
        List<int> productIdsInRequest = requestDto.Items
            .OrderBy(i => i.ProductId)
            .Select(i => i.ProductId)
            .ToList();
        List<Product> products = await _context.Products
            .Where(p => productIdsInRequest.Contains(p.Id))
            .ToListAsync();
        HashSet<int> fetchedProductIds = products.Select(p => p.Id).ToHashSet();
        for (int i = 0; i < productIdsInRequest.Count; i++)
        {
            int idInRequest = productIdsInRequest[i];
            if (!fetchedProductIds.Contains(idInRequest))
            {
                string errorMessage = ErrorMessages.NotFoundByProperty
                    .ReplaceResourceName(DisplayNames.Product)
                    .ReplacePropertyName(DisplayNames.Id)
                    .ReplaceAttemptedValue(idInRequest.ToString());
                throw new OperationException($"items[{i}].productId", errorMessage);
            }
        }

        // Initialize supply.
        Supply supply = new Supply
        {
            SuppliedDateTime = requestDto.SuppliedDateTime ?? DateTime.UtcNow.ToApplicationTime(),
            ShipmentFee = requestDto.ShipmentFee,
            Note = requestDto.Note,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            CreatedUserId = _authorizationService.GetUserId(),
            Items = new List<SupplyItem>(),
            Photos = new List<SupplyPhoto>()
        };

        // Initialize items
        foreach (SupplyItemRequestDto itemRequestDto in requestDto.Items)
        {
            Product product = products.Single(i => i.Id == itemRequestDto.ProductId);
            SupplyItem supplyItem = new()
            {
                Amount = itemRequestDto.Amount,
                SuppliedQuantity = itemRequestDto.SuppliedQuantity,
                ProductId = product.Id
            };
            product.StockingQuantity += itemRequestDto.SuppliedQuantity;
            supply.Items.Add(supplyItem);
        }

        // Initialize photos
        if (requestDto.Photos != null)
        {
            foreach (SupplyPhotoRequestDto photoRequestDto in requestDto.Photos)
            {
                string url = await _photoService
                    .CreateAsync(photoRequestDto.File, "supplies", false);
                SupplyPhoto supplyPhoto = new()
                {
                    Url = url
                };
                supply.Photos.Add(supplyPhoto);
            }
        }

        // Save changes and handle errors.
        _context.Supplies.Add(supply);
        try
        {
            await _context.SaveChangesAsync();
            await _statsService.IncrementSupplyCostAsync(supply.ItemAmount);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee);
            await transaction.CommitAsync();
            return supply.Id;
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException)
        {
            await transaction.RollbackAsync();
            // Delete all created photos.
            foreach (SupplyPhoto supplyPhoto in supply.Photos)
            {
                _photoService.Delete(supplyPhoto.Url);
            }

            HandleCreateOrUpdateException(exception.InnerException as MySqlException);
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
            .Include(s => s.UpdateHistories)
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
        long originalItemAmount = supply.ItemAmount;
        long originalShipmentFee = supply.ShipmentFee;

        // Update supply properties.
        supply.SuppliedDateTime = requestDto.SuppliedDateTime ?? DateTime.UtcNow.ToApplicationTime();
        supply.ShipmentFee = requestDto.ShipmentFee;
        supply.Note = requestDto.Note;

        // Update supply items.
        UpdateItems(supply, requestDto.Items);

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

        // Create update history.
        CreateUpdateHistory(supply, oldData, newData, requestDto.UpdateReason);

        // Perform the updating operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The supply can be saved without any error, adjust the stats.
            await _statsService.IncrementSupplyCostAsync(supply.ItemAmount - originalItemAmount);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee - originalShipmentFee);
            
            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();
            foreach (string url in urlsToBeDeletedWhenSucceed)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            await transaction.RollbackAsync();
            foreach (string url in urlsToBeDeletedWhenFails)
            {
                _photoService.Delete(url);
            }
            HandleCreateOrUpdateException(sqlException);
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

        try
        {
            long originalItemAmount = supply.ItemAmount;
            long originalShipmentFee = supply.ShipmentFee;
            _context.Supplies.Remove(supply);
            await _context.SaveChangesAsync();
            // Adjust product stocking quantity.
            foreach (SupplyItem item in supply.Items)
            {
                item.Product.StockingQuantity -= item.SuppliedQuantity;
            }
            // Adjust stats.
            await _statsService.IncrementSupplyCostAsync(-originalItemAmount);
            await _statsService.IncrementSupplyCostAsync(-originalShipmentFee);
            // Commit transaction.
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
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException)
        {
            HandleDeleteExeption(exception.InnerException as MySqlException);
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
    private static void  HandleDeleteExeption(MySqlException exception)
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
    /// Update the specified supply's items with the data provided in the request.
    /// </summary>
    /// <param name="supply">
    /// The supply associated to the items to be updated.
    /// </param>
    /// <param name="requestDtos">
    /// An object containing data for the items to be updated.
    /// </param>
    private static void UpdateItems(
            Supply supply,
            List<SupplyItemRequestDto> requestDtos)
    {
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
    /// Update the specified supply's photos with the data provided in the request.
    /// </summary>
    /// <param name="supply">
    /// The supply associated to the items to be updated.
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
    /// Create and add update history for the specified supply.
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
    private static void CreateUpdateHistory(
            Supply supply,
            SupplyUpdateHistoryDataDto oldData,
            SupplyUpdateHistoryDataDto newData,
            string reason)
    {
        SupplyUpdateHistory updateHistory = new SupplyUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData)
        };
        
        supply.UpdateHistories ??= new List<SupplyUpdateHistory>();
        supply.UpdateHistories.Add(updateHistory);
    }
}
