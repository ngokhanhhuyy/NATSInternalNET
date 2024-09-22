namespace NATSInternal.Services;

/// <inheritdoc />
public class OrderService : LockableEntityService, IOrderService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
    private static MonthYearResponseDto _earliestRecordedMonthYear { get; set; }

    public OrderService(
        DatabaseContext context,
        IPhotoService photoService,
        IAuthorizationService authorizationService,
        IStatsService statsService)
    {
        _context = context;
        _photoService = photoService;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<OrderListResponseDto> GetListAsync(OrderListRequestDto requestDto)
    {
        // Initialize list of month and year options.
        List<MonthYearResponseDto> monthYearOptions = null;
        if (!requestDto.IgnoreMonthYear)
        {
            _earliestRecordedMonthYear ??= await _context.Orders
                .OrderBy(s => s.PaidDateTime)
                .Select(s => new MonthYearResponseDto
                {
                    Year = s.PaidDateTime.Year,
                    Month = s.PaidDateTime.Month
                }).FirstOrDefaultAsync();
            monthYearOptions = GenerateMonthYearOptions(_earliestRecordedMonthYear);
        }

        // Initialize query.
        IQueryable<Order> query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.CreatedUser).ThenInclude(u => u.Roles)
            .Include(o => o.Items)
            .Include(o => o.Photos);

        // Sort by the specified direction and field.
        switch (requestDto.OrderByField)
        {
            case nameof(OrderListRequestDto.FieldOptions.Amount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(o => o.Items.Sum(i => i.Amount))
                        .ThenBy(o => o.PaidDateTime)
                    : query.OrderByDescending(o => o.Items.Sum(i => i.Amount))
                        .ThenByDescending(o => o.PaidDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(o => o.PaidDateTime)
                        .ThenBy(o => o.Items.Sum(i => i.Amount))
                    : query.OrderByDescending(o => o.PaidDateTime)
                        .ThenByDescending(o => o.Items.Sum(i => i.Amount));
                break;
        }

        // Filter by month and year if specified.
        if (!requestDto.IgnoreMonthYear)
        {
            DateTime startDateTime;
            startDateTime = new DateTime(requestDto.Year, requestDto.Month, 1);
            DateTime endDateTime = startDateTime.AddMonths(1);
            query = query
                .Where(o => o.PaidDateTime >= startDateTime && o.PaidDateTime < endDateTime);
        }

        // Filter by user id if specified.
        if (requestDto.UserId.HasValue)
        {
            query = query.Where(o => o.CreatedUserId == requestDto.UserId);
        }

        // Filter by customer id if specified.
        if (requestDto.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == requestDto.CustomerId);
        }

        // Filter by product id if specified.
        if (requestDto.ProductId.HasValue)
        {
            query = query.Where(o => o.Items.Any(oi => oi.ProductId == requestDto.ProductId));
        }

        // Filter by not being soft deleted.
        query = query.Where(o => !o.IsDeleted);

        // Initialize response dto.
        OrderListResponseDto responseDto = new OrderListResponseDto
        {
            MonthYearOptions = monthYearOptions,
            Authorization = _authorizationService.GetOrderListAuthorization()
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
            .Select(o => new OrderBasicResponseDto(
                o,
                _authorizationService.GetOrderAuthorization(o)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<OrderDetailResponseDto> GetDetailAsync(int id)
    {
        // Initialize query.
        IQueryable<Order> query = _context.Orders
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .Include(o => o.Photos)
            .Include(o => o.Customer)
            .Include(o => o.CreatedUser).ThenInclude(u => u.Roles);

        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessOrderUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(o => o.UpdateHistories);
        }

        // Fetch the entity with the given id from the database.
        Order order = await query
            .AsSplitQuery()
            .SingleOrDefaultAsync(o => o.Id == id && !o.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(User),
                nameof(id),
                id.ToString());

        return new OrderDetailResponseDto(
            order,
            _authorizationService.GetOrderAuthorization(order),
            mapUpdateHistories: shouldIncludeUpdateHistories);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(OrderUpsertRequestDto requestDto)
    {
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Determine ordered datetime.
        DateTime orderedDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the ordered datetime.
            if (!_authorizationService.CanSetOrderPaidDateTime())
            {
                throw new AuthorizationException();
            }

            // The ordered datetime is valid, assign it to the new order.
            orderedDateTime = requestDto.PaidDateTime.Value;
        }

        // Initialize order entity.
        Order order = new Order
        {
            PaidDateTime = orderedDateTime,
            Note = requestDto.Note,
            CustomerId = requestDto.CustomerId,
            CreatedUserId = _authorizationService.GetUserId(),
            Items = new List<OrderItem>(),
            Photos = new List<OrderPhoto>()
        };
        _context.Orders.Add(order);

        // Initialize order items entities.
        await CreateItems(order, requestDto.Items);

        // Initialize photos.
        if (requestDto.Photos != null)
        {
            await CreatePhotosAsync(order, requestDto.Photos);
        }

        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();

            // The order can be created successfully without any error. Add the order
            // to the stats.
            DateOnly orderedDate = DateOnly.FromDateTime(order.PaidDateTime);
            await _statsService.IncrementRetailGrossRevenueAsync(order.BeforeVatAmount, orderedDate);
            if (order.VatAmount > 0)
            {
                await _statsService.IncrementVatCollectedAmountAsync(order.VatAmount, orderedDate);
            }

            // Commit the transaction, finish the operation.
            await transaction.CommitAsync();
            return order.Id;
        }
        catch (DbUpdateException exception)
        {
            // Remove all the created photos.
            foreach (OrderPhoto photo in order.Photos)
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
                        case "user_id":
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
    public async Task UpdateAsync(int id, OrderUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Order order = await _context.Orders
            .Include(o => o.CreatedUser)
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .Include(o => o.Photos)
            .SingleOrDefaultAsync(o => o.Id == id && !o.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Order),
                nameof(id),
                id.ToString());

        // Check if the current user has permission to edit this order.
        if (!_authorizationService.CanEditOrder(order))
        {
            throw new AuthorizationException();
        }

        // Use transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Storing the old data for update history logging and stats adjustments.
        long oldItemAmount = order.BeforeVatAmount;
        long oldVatAmount = order.VatAmount;
        DateOnly oldPaidDate = DateOnly.FromDateTime(order.PaidDateTime);
        OrderUpdateHistoryDataDto oldData = new OrderUpdateHistoryDataDto(order);

        // Handle the new ordered datetime when the request specifies it.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify a new ordered datetime.
            if (!_authorizationService.CanSetOrderPaidDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the order's PaidDateTime to be modified when the order is locked.
            if (order.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Order)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(
                    nameof(requestDto.PaidDateTime),
                    errorMessage);
            }

            // Assign the new PaidDateTime value only if it's different from the old one.
            if (requestDto.PaidDateTime.Value != order.PaidDateTime)
            {
                // Validate the specified PaidDateTime value from the request.
                try
                {
                    _statsService.ValidateStatsDateTime(order, requestDto.PaidDateTime.Value);
                    order.PaidDateTime = requestDto.PaidDateTime.Value;
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

        // Update order properties.
        order.Note = requestDto.Note;

        // Update order items.
        await UpdateItems(order, requestDto.Items);

        // Update photos.
        List<string> urlsToBeDeletedWhenSucceeds = new List<string>();
        List<string> urlsToBeDeletedWhenFails = new List<string>();
        if (requestDto.Photos != null)
        {
            (List<string>, List<string>) photoUpdateResults;
            photoUpdateResults = await UpdatePhotosAsync(order, requestDto.Photos);
            urlsToBeDeletedWhenSucceeds.AddRange(photoUpdateResults.Item1);
            urlsToBeDeletedWhenFails.AddRange(photoUpdateResults.Item2);
        }

        // Store new data for update history logging.
        OrderUpdateHistoryDataDto newData = new OrderUpdateHistoryDataDto(order);

        // Log update history.
        LogUpdateHistory(order, oldData, newData, requestDto.UpdateReason);

        // Save changes and handle errors.
        try
        {
            // Save all modifications.
            await _context.SaveChangesAsync();

            // The order can be updated successfully without any error.
            // Adjust the stats for items' amount and vat collected amount.
            // Revert the old stats.
            await _statsService.IncrementRetailGrossRevenueAsync(-oldItemAmount, oldPaidDate);
            await _statsService.IncrementVatCollectedAmountAsync(-oldVatAmount, oldPaidDate);

            // Delete all old photos which have been replaced by new ones.
            DateOnly newPaidDate = DateOnly.FromDateTime(order.PaidDateTime);
            await _statsService
                .IncrementRetailGrossRevenueAsync(order.BeforeVatAmount, newPaidDate);
            await _statsService.IncrementVatCollectedAmountAsync(order.VatAmount, newPaidDate);

            // Delete photo files which have been specified.
            foreach (string url in urlsToBeDeletedWhenSucceeds)
            {
                _photoService.Delete(url);
            }

            // Commit the trasaction and finish the operation.
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
                    throw new ResourceNotFoundException(
                        nameof(Customer),
                        nameof(requestDto.CustomerId),
                        requestDto.CustomerId.ToString());
                }
            }
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure it exists.
        Order order = await _context.Orders
            .SingleOrDefaultAsync(o => o.Id == id && !o.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Order),
                nameof(id),
                id.ToString());

        // Check if the current user has permission to delete the order.
        if (!_authorizationService.CanDeleteOrder(order))
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
            // Handle concurrency exception.
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
                    // Soft delete when there are any other related entities which are restricted to be deleted.
                    order.IsDeleted = true;

                    // Save changes.
                    await _context.SaveChangesAsync();

                    // Order has been deleted successfully, adjust the stats.
                    DateOnly orderedDate = DateOnly.FromDateTime(order.PaidDateTime);
                    await _statsService.IncrementRetailGrossRevenueAsync(
                        order.BeforeVatAmount,
                        orderedDate);
                    await _statsService.IncrementVatCollectedAmountAsync(
                        order.VatAmount,
                        orderedDate);

                    // Commit the transaction and finishing the operations.
                    await transaction.CommitAsync();
                }
            }
            throw;
        }
    }

    /// <summary>
    /// Create order items associated to the given order with the data provided
    /// in the request. This method must only be called during the order creating operation.
    /// </summary>
    /// <param name="order">
    /// The order which items are to be created.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the new data for the creating operation.
    /// </param>
    private async Task CreateItems(Order order, List<OrderItemRequestDto> requestDtos)
    {
        // Fetch a list of products which ids are specified in the request.
        List<int> requestedProductIds = requestDtos
            .Select(dto => dto.ProductId)
            .ToList();
        List<Product> products = await _context.Products
            .Where(p => requestedProductIds.Contains(p.Id))
            .ToListAsync();

        for (int i = 0; i < requestDtos.Count; i++)
        {
            OrderItemRequestDto itemRequestDto = requestDtos[i];

            // Get the product from the pre-fetched list.
            Product product = products.SingleOrDefault(p => p.Id == itemRequestDto.ProductId);

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
            OrderItem item = new OrderItem
            {
                Amount = itemRequestDto.Amount,
                VatFactor = itemRequestDto.VatFactor,
                Quantity = itemRequestDto.Quantity,
                Product = product
            };

            // Adjust the stocking quantity of the associated product.
            product.StockingQuantity -= item.Quantity;

            // Add item.
            order.Items.Add(item);
        }
    }

    /// <summary>
    /// Update or create order items associated to the given order with the data provided
    /// in the request. This method must only be called during the order updating operation.
    /// </summary>
    /// <param name="order">The order which items are to be created or updated.</param>
    /// <param name="requestDtos">A list of objects containing the new data for updating operation.</param>
    /// <returns>A <c>Task</c> object representing the asynchronous operation.</returns>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    private async Task UpdateItems(Order order, List<OrderItemRequestDto> requestDtos)
    {
        // Fetch a list of products which ids are specified in the request for the new items.
        List<int> productIdsForNewItems = requestDtos
            .Where(dto => !dto.Id.HasValue)
            .Select(dto => dto.ProductId)
            .ToList();
        List<Product> productsForNewItems = await _context.Products
            .Where(p => productIdsForNewItems.Contains(p.Id))
            .ToListAsync();

        for (int i = 0; i < requestDtos.Count; i++)
        {
            OrderItemRequestDto itemRequestDto = requestDtos[i];
            OrderItem item;
            if (itemRequestDto.Id.HasValue)
            {
                item = order.Items.SingleOrDefault(oi => oi.Id == itemRequestDto.Id.Value);

                // Ensure the item exists.
                if (item == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.OrderItem);
                    throw new OperationException($"items[{i}].id", errorMessage);
                }

                // Revert the stocking quantity of the product associated to the item.
                item.Product.StockingQuantity += item.Quantity;

                // Remove item if deleted.
                if (itemRequestDto.HasBeenDeleted)
                {
                    _context.OrderItems.Remove(item);
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
                // Get the product entity in the pre-fetched list.
                Product product = productsForNewItems
                    .SingleOrDefault(p => p.Id == itemRequestDto.ProductId);

                // Ensure the product exists.
                if (product == null)
                {
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplaceResourceName(DisplayNames.Product)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(itemRequestDto.ProductId.ToString());
                    throw new OperationException(
                        $"items[{i}].productId",
                        errorMessage);
                }

                // Initialize the entity.
                item = new OrderItem
                {
                    Amount = itemRequestDto.Amount,
                    VatFactor = itemRequestDto.VatFactor,
                    Quantity = itemRequestDto.Quantity,
                    Product = product,
                    Order = order
                };
                _context.OrderItems.Add(item);
            }

            // Validate the new quantity value.
            if (item.Product.StockingQuantity - item.Quantity < 0)
            {
                string errorMessage = ErrorMessages.NegativeProductStockingQuantity;
                throw new OperationException($"items[{i}].stockingQuantity", errorMessage);
            }

            // Add the quantity of the item to the product's stocking quantity.
            item.Product.StockingQuantity -= item.Quantity;
        }
    }

    /// <summary>
    /// Creates photos which are associated with the specified order.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the order creating operation.
    /// </remarks>
    /// <param name="order">
    /// An instance of the <see cref="Order"/> entity class, representing the order with which
    /// the creating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is the instances of the
    /// <see cref="OrderPhotoRequestDto"/> class, containing the data for the creating
    /// operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    private async Task CreatePhotosAsync(Order order, List<OrderPhotoRequestDto> requestDtos)
    {
        foreach (OrderPhotoRequestDto photoRequestDto in requestDtos)
        {
            string url = await _photoService.CreateAsync(photoRequestDto.File, "orders", false);
            OrderPhoto photo = new OrderPhoto
            {
                Url = url
            };
            order.Photos.Add(photo);
        }
    }

    /// <summary>
    /// Updates or creates new photos which are associated with the specified order.
    /// </summary>
    /// <remarks>
    /// This method must be called during the order updating operation.
    /// </remarks>
    /// <param name="order">
    /// An instance of the <see cref="Order"/> entity class, which represents the order with
    /// which the updating or creating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A <see cref="List{T}"/> where <c>T</c> is <see cref="OrderPhotoRequestDto"/>,
    /// containing the data for the updating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Tuple"/> containing two <see cref="List{T}"/> where <c>T</c> is
    /// <see cref="string"/>. The first one represents the deleted photos' urls which must be
    /// deleted after the whole order updating operation succeeds, the second one represents
    /// the created photos' urls which must be deleted after the whole order updating operation
    /// fails.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws when a photo with its specified id is indicated to be updated but doesn't exist
    /// or has already been deleted.
    /// </exception>
    private async Task<(List<string>, List<string>)> UpdatePhotosAsync(
            Order order,
            List<OrderPhotoRequestDto> requestDtos)
    {
        List<string> urlsToBeDeletedWhenSucceeds = new List<string>();
        List<string> urlsToBeDeletedWhenFails = new List<string>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            OrderPhotoRequestDto requestDto = requestDtos[i];
            OrderPhoto photo;
            if (requestDto.Id.HasValue)
            {
                // Fetch the photo entity with the given id from the request.
                photo = order.Photos.SingleOrDefault(op => op.Id == requestDto.Id);

                // Ensure the photo entity exists.
                if (photo == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.Photo)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(requestDto.Id.ToString());
                    throw new OperationException($"photos[{i}]", errorMessage);
                }

                // Perform the modification when the photo is marked to have been changed
                if (requestDto.HasBeenChanged)
                {
                    // Mark the current url to be deleted later when the transaction succeeds.
                    urlsToBeDeletedWhenSucceeds.Add(photo.Url);

                    // Create new photo if the request contains new data for a new one.
                    if (requestDto.File != null)
                    {
                        string url = await _photoService
                            .CreateAsync(requestDto.File, "orders", true);
                        photo.Url = url;

                        // Mark the created photo to be deleted later if the transaction fails.
                        urlsToBeDeletedWhenFails.Add(url);
                    }
                }
            }
            else
            {
                // Create new photo if the request doesn't have id.
                string url = await _photoService.CreateAsync(requestDto.File, "orders", true);
                photo = new OrderPhoto
                {
                    Url = url
                };
                order.Photos.Add(photo);

                // Mark the created photo to be deleted later if the transaction fails.
                urlsToBeDeletedWhenFails.Add(url);
            }
        }

        return (urlsToBeDeletedWhenSucceeds, urlsToBeDeletedWhenFails);
    }

    /// <summary>
    /// Logs the old and new data to update history for the specified order.
    /// </summary>
    /// <remarks>
    /// This method must only be called during the updating operation of an order.
    /// </remarks>
    /// <param name="order">
    /// An instance of the <see cref="Order"/> entity class, representing the order to be
    /// logged.
    /// </param>
    /// <param name="oldData">
    /// An instance of the <see cref="OrderUpdateHistoryDataDto"/> class, containing the data
    /// of the order before the modification.
    /// </param>
    /// <param name="newData">
    /// An instance of the <see cref="OrderUpdateHistoryDataDto"/> class, containing the data
    /// of the order after the modification.
    /// </param>
    /// <param name="reason">
    /// A <see cref="string"/> value representing the reason of the modification.
    /// </param>
    private void LogUpdateHistory(
            Order order,
            OrderUpdateHistoryDataDto oldData,
            OrderUpdateHistoryDataDto newData,
            string reason)
    {
        OrderUpdateHistory updateHistory = new OrderUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId()
        };

        order.UpdateHistories ??= new List<OrderUpdateHistory>();
        order.UpdateHistories.Add(updateHistory);
    }
}