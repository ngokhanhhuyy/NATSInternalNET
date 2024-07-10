using System.Linq;

namespace NATSInternal.Services;

/// <inheritdoc />
public class OrderService : IOrderService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
    private readonly IOrderPaymentService _orderPaymentService;
        
    public OrderService(
        DatabaseContext context,
        IPhotoService photoService,
        IAuthorizationService authorizationService,
        IStatsService statsService,
        IOrderPaymentService orderPaymentService)
    {
        _context = context;
        _photoService = photoService;
        _authorizationService = authorizationService;
        _statsService = statsService;
        _orderPaymentService = orderPaymentService;
    }

    /// <inheritdoc />
    public async Task<OrderListResponseDto> GetListAsync(OrderListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<Order> query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.User).ThenInclude(u => u.Roles)
            .Include(o => o.Items)
            .Include(o => o.Photos);
            
        // Sorting direction and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(OrderListRequestDto.FieldOptions.Amount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(o => o.Items.Sum(i => i.Amount))
                        .ThenBy(o => o.OrderedDateTime)
                    : query.OrderByDescending(o => o.Items.Sum(i => i.Amount))
                        .ThenByDescending(o => o.OrderedDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(o => o.OrderedDateTime)
                        .ThenBy(o => o.Items.Sum(i => i.Amount))
                    : query.OrderByDescending(o => o.OrderedDateTime)
                        .ThenByDescending(o => o.Items.Sum(i => i.Amount));
                break;
        }
            
        // Filter from range if specified.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(o => o.OrderedDateTime >= rangeFromDateTime);
        }
            
        // Filter to range if specified.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value, new TimeOnly(0, 0, 0));
            query = query.Where(o => o.OrderedDateTime <= rangeToDateTime);
        }

        // Specify split query for better performance.
        query = query.AsSingleQuery();
            
        // Initialize response dto.
        OrderListResponseDto responseDto = new OrderListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(o => new OrderBasicResponseDto
            {
                Id = o.Id,
                OrderedDateTime = o.OrderedDateTime,
                Amount = o.ItemAmount,
                IsClosed = o.IsClosed,
                Customer = new CustomerBasicResponseDto
                {
                    Id = o.Customer.Id,
                    FullName = o.Customer.FullName,
                    NickName = o.Customer.NickName,
                    Gender = o.Customer.Gender,
                    Birthday = o.Customer.Birthday,
                    PhoneNumber = o.Customer.PhoneNumber
                },
                Authorization = _authorizationService.GetOrderAuthorization(o)
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        responseDto.Authorization = _authorizationService.GetOrderListAuthorization();
        return responseDto;
    }

    /// <inheritdoc />
    public async Task<OrderDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .Include(o => o.Payments).ThenInclude(op => op.User).ThenInclude(u => u.Roles)
            .Include(o => o.Photos)
            .Include(o => o.Customer)
            .Include(o => o.User)
            .Where(o => o.Id == id)
            .Select(o => new OrderDetailResponseDto
            {
                Id = o.Id,
                OrderedDateTime = o.OrderedDateTime,
                ItemAmount = o.ItemAmount,
                PaidAmount = o.PaidAmount,
                Note = o.Note,
                IsClosed = o.IsClosed,
                Items = o.Items
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        Amount = oi.Amount,
                        VatFactor = oi.VatFactor,
                        Quantity = oi.Quantity,
                        Product = new ProductBasicResponseDto
                        {
                            Id = oi.Product.Id,
                            Name = oi.Product.Name,
                            Unit = oi.Product.Unit,
                            Price = oi.Product.Price,
                            StockingQuantity = oi.Product.StockingQuantity,
                            ThumbnailUrl = oi.Product.ThumbnailUrl
                        }
                    }).ToList(),
                Payments = o.Payments
                    .Select(op => new OrderPaymentResponseDto
                    {
                        Id = op.Id,
                        Amount = op.Amount,
                        PaidDateTime = op.PaidDateTime,
                        Note = op.Note,
                        IsClosed = op.IsClosed,
                        UserInCharge = new UserBasicResponseDto
                        {
                            Id = op.User.Id,
                            UserName = op.User.UserName,
                            FirstName = op.User.FirstName,
                            MiddleName = op.User.MiddleName,
                            LastName = op.User.LastName,
                            FullName = op.User.FullName,
                            Gender = op.User.Gender,
                            Birthday = op.User.Birthday,
                            JoiningDate = op.User.JoiningDate,
                            AvatarUrl = op.User.AvatarUrl,
                            Role = new RoleBasicResponseDto
                            {
                                Id = op.User.Role.Id,
                                Name = op.User.Role.Name,
                                DisplayName = op.User.Role.DisplayName,
                                PowerLevel = op.User.Role.PowerLevel
                            },
                        },
                        Authorization = _authorizationService.GetOrderPaymentAuthorization(op)
                    }).ToList(),
                Customer = new CustomerBasicResponseDto
                {
                    Id = o.Customer.Id,
                    FullName = o.Customer.FullName,
                    NickName = o.Customer.NickName,
                    Gender = o.Customer.Gender,
                    Birthday = o.Customer.Birthday,
                    PhoneNumber = o.Customer.PhoneNumber
                },
                User = new UserBasicResponseDto
                {
                    Id = o.User.Id,
                    UserName = o.User.UserName,
                    FirstName = o.User.FirstName,
                    MiddleName = o.User.MiddleName,
                    LastName = o.User.LastName,
                    FullName = o.User.FullName,
                    Gender = o.User.Gender,
                    Birthday = o.User.Birthday,
                    JoiningDate = o.User.JoiningDate,
                    AvatarUrl = o.User.AvatarUrl,
                    Role = new RoleBasicResponseDto
                    {
                        Id = o.User.Role.Id,
                        Name = o.User.Role.Name,
                        DisplayName = o.User.Role.DisplayName,
                        PowerLevel = o.User.Role.PowerLevel
                    },
                },
                Photos = o.Photos
                    .Select(op => new OrderPhotoResponseDto
                    {
                        Id = op.Id,
                        Url = op.Url
                    }).ToList(),
                Authorization = _authorizationService.GetOrderAuthorization(o)
            }).AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(User),
                nameof(id),
                id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(OrderUpsertRequestDto requestDto)
    {
        // Using transaction for atomic operations.
        using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Initialize order entity.
        Order order = new Order
        {
            OrderedDateTime = requestDto.OrderedDateTime
                ?? DateTime.UtcNow.ToApplicationTime(),
            Note = requestDto.Note,
            CustomerId = requestDto.CustomerId,
            UserId = _authorizationService.GetUserId(),
            Items = new List<OrderItem>(),
            Photos = new List<OrderPhoto>(),
            Payments = new List<OrderPayment>(),
        };
        _context.Orders.Add(order);

        // Initialize order items entities.
        CreateItems(order, requestDto.Items);

        // Initialize order payment entity.
        if (requestDto.Payment != null)
        {
            await _orderPaymentService.CreateAsync(order, requestDto.Payment, "payment");
        }
        
        // Initialize photos.
        if (requestDto.Photos != null)
        {
        }
        
        try
        {
            await _context.SaveChangesAsync();
            await _statsService.IncrementRetailRevenueAsync(
                order.ItemAmount,
                DateOnly.FromDateTime(order.OrderedDateTime));
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
            .Include(o => o.User)
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Photos)
            .SingleOrDefaultAsync(o => o.Id == id)
            ?? throw new ResourceNotFoundException(nameof(Order), nameof(id), id.ToString());
        
        // Check if the current user has permission to edit this order.
        if (!_authorizationService.CanEditOrder(order))
        {
            throw new AuthorizationException();
        }

        // Check if ordered datetime is valid.
        if (requestDto.OrderedDateTime.HasValue)
        {
            if (!IsUpdatedOrderedDateTimeValid(order, requestDto.OrderedDateTime.Value))
            {
                throw new OperationException(
                    nameof(requestDto.OrderedDateTime),
                    ErrorMessages.Invalid.ReplacePropertyName(DisplayNames.OrderedDateTime));
            }
        }
        
        // Revert stats for amount if changed.
        if (order.ItemAmount != requestDto.Items.Where(i => !i.HasBeenDeleted).Sum(i => i.Amount))
        {
            await _statsService.IncrementRetailRevenueAsync(order.ItemAmount);
        }

        // Update order properties.
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        order.OrderedDateTime = requestDto.OrderedDateTime ?? currentDateTime;
        order.Note = requestDto.Note;
        order.CustomerId = requestDto.CustomerId;

        // Update order items.
        UpdateItems(order, requestDto.Items);

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

        // Save changes and catch errors.
        try
        {
            // Save all modifications.
            await _context.SaveChangesAsync();

            // No error occured during the saving operation, finishing the transaction.
            // Delete all old photos which have been replaced by new ones.
            foreach (string url in urlsToBeDeletedWhenSucceeds)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        {
            // Undo all the created photos.
            foreach (string url in urlsToBeDeletedWhenFails)
            {
                _photoService.Delete(url);
            }

            // Handle concurrency exception.
            if (exception.InnerException is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handling the exception in the foreseen cases.
            else if (exception.InnerException is MySqlException sqlException)
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
            .Include(o => o.Payments)
            .Where(o => !o.IsDeleted)
            .SingleOrDefaultAsync(o => o.Id == id)
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
            _context.Orders.Remove(order);
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

                    // Deleted the order successfully, adjust the stats.
                    await _statsService.IncrementRetailRevenueAsync(
                        order.PaidAmount,
                        DateOnly.FromDateTime(order.OrderedDateTime));

                    // Commit the transaction and finishing the operations.
                    await transaction.CommitAsync();
                }
            }
            throw;
        }
    }

    /// <summary>
    /// Check if the ordered datetime value provided in the request is valid to be updated
    /// for the given order.
    /// </summary>
    /// <param name="order">The order to be updated.</param>
    /// <param name="newOrderedDateTime">The new ordered datetime provided in the request.</param>
    /// <returns>
    /// <c>true</c> if the ordered datetime value is valid; otherwise, <c>false</c>.
    /// </returns>
    private bool IsUpdatedOrderedDateTimeValid(Order order, DateTime newOrderedDateTime)
    {
        DateTime minDateTime = new DateTime(
            order.OrderedDateTime.AddMonths(-1).Year,
            order.OrderedDateTime.AddMonths(-1).Month,
            1,
            0, 0, 0);
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime maxDateTime = order.OrderedDateTime.AddMonths(2) > currentDateTime
            ? currentDateTime
            : order.OrderedDateTime.AddMonths(2);
        return newOrderedDateTime > minDateTime || newOrderedDateTime <= maxDateTime;
    }

    /// <summary>
    /// Check if the order should be closed with given ordered datetime. The order is closed if
    /// the ordered datetime belongs to the month of 2 months ago.
    /// </summary>
    /// <param name="dateTime">The datetime of the order to be checked.</param>
    /// <returns>
    /// <c>true</c> if the ordered datetime belongs to the month of 2 months ago.;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// If the current datetime is 2024-01-01T00:00:00, the order should be closed if
    /// the ordered datetime is earlier than 2023-10-31T23:59:59.
    /// </example>
    private bool ShouldOrderBeClosedByDateTime(DateTime dateTime)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime().AddMonths(-2);
        DateTime minimumAllowedDateTime = new DateTime(
            currentDateTime.Year, currentDateTime.Month, 1,
            0, 0, 0);
        return dateTime < minimumAllowedDateTime;
    }

    /// <summary>
    /// Create order items associated to the given order with the data provided
    /// in the request. This method must only be called during the order creating operation.
    /// </summary>
    /// <param name="order">The order which items are to be created.</param>
    /// <param name="requestDtos">A list of objects containing the new data for the creating operation</param>
    private void CreateItems(Order order, List<OrderItemRequestDto> requestDtos)
    {
        foreach (OrderItemRequestDto itemRequestDto in requestDtos)
        {
            OrderItem item = new OrderItem
            {
                Amount = itemRequestDto.Amount,
                VatFactor = itemRequestDto.VatFactor,
                Quantity = itemRequestDto.Quantity,
                ProductId = itemRequestDto.ProductId
            };
            order.Items.Add(item);
        }
    }

    /// <summary>
    /// Update or create order items associated to the given order with the data provided
    /// in the request. This method must only be called during the order updating operation.
    /// </summary>
    /// <param name="order">The order which items are to be created or updated.</param>
    /// <param name="requestDtos">A list of objects containing the new data for updating operation.</param>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    private void UpdateItems(Order order, List<OrderItemRequestDto> requestDtos)
    {
        for (int i = 0; i < requestDtos.Count; i++)
        {
            OrderItemRequestDto itemRequestDto = requestDtos[i];
            OrderItem item;
            if (itemRequestDto.Id.HasValue)
            {
                item = order.Items.SingleOrDefault(i => i.Id == itemRequestDto.Id.Value);

                // Throw error if the item couldn't be found.
                if (item == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.OrderItem);
                    throw new OperationException($"items[{i}].id", errorMessage);
                }

                // Remove item if deleted.
                if (itemRequestDto.HasBeenDeleted)
                {
                    _context.OrderItems.Remove(item);
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
                item = new OrderItem
                {
                    Amount = itemRequestDto.Amount,
                    VatFactor = itemRequestDto.VatFactor,
                    Quantity = itemRequestDto.Quantity,
                    ProductId = itemRequestDto.ProductId
                };
                _context.OrderItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Create order photos associated to the given order with the database provided in the request.
    /// This method must only be called during the order creating operation.
    /// </summary>
    /// <param name="order">The order which photos are to be created.</param>
    /// <param name="requestDtos">A list of objects containing the data for the creating operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
    /// Update or create order photos associated to the given order with the data provided
    /// in the request. This method must only be called during the order updating operation.
    /// </summary>
    /// <param name="order">The order which photos are to be created or updated.</param>
    /// <param name="requestDtos">A list of objects containing the new data for updating operation.</param>
    /// <returns>
    /// A tuple containing 2 lists of photos' url strings, the first one represents the deleted photos'
    /// urls which must be deleted after the whole order updating operation succeeds, the second one
    /// represents the  created photos' urls which must be deleted after the whole order updating operation fails.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
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
                        string url = await _photoService.CreateAsync(requestDto.File, "orders", true);
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

                // Mark the created photo to be deleted later if the transaction fails.
                urlsToBeDeletedWhenFails.Add(url);
            }
        }

        return (urlsToBeDeletedWhenSucceeds, urlsToBeDeletedWhenFails);
    }
}