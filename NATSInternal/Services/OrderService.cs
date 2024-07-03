namespace NATSInternal.Services;

/// <inheritdoc />
public class OrderService : IOrderService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
        
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
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        return responseDto;
    }
    
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
                        }
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
                    }).ToList()
            }).AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(User),
                nameof(id),
                id.ToString());
    }
    
    public async Task<int> CreateAsync(OrderUpsertRequestDto requestDto)
    {
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
        foreach (OrderItemRequestDto itemRequestDto in requestDto.Items)
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

        // Initialize order payment entity.
        if (requestDto.Payment != null)
        {
            OrderPayment payment = new OrderPayment
            {
                Amount = requestDto.Payment.Amount,
                PaidDateTime = requestDto.Payment.PaidDateTime
                    ?? DateTime.UtcNow.ToApplicationTime(),
                Note = requestDto.Payment.Note
            };
            order.Payments.Add(payment);
        }
        
        // Initialize photos.
        if (requestDto.Photos != null)
        { 
            foreach (OrderPhotoRequestDto photoRequestDto in requestDto.Photos)
            {
                string url = await _photoService
                    .CreateAsync(photoRequestDto.File, "orders", false);
                OrderPhoto photo = new OrderPhoto
                {
                    Url = url
                };
                order.Photos.Add(photo);
            }
        }
        
        // Begin transactions and save changes.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        try
        {
            await _context.SaveChangesAsync();
            await _statsService.IncrementRetailRevenueAsync(order.Items.Sum(i => i.Amount));
            await transaction.CommitAsync();
            return order.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
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
            throw;
        }
        catch (DbUpdateException)
        {
            throw new ConcurrencyException();
        }
    }
    
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
        
        // Revert stats for amount if changed.
        if (order.ItemAmount != requestDto.Items.Where(i => !i.HasBeenDeleted).Sum(i => i.Amount))
        {
            await _statsService.IncrementRetailRevenueAsync(order.ItemAmount);
        }
        
        // Update order properties.
        order.OrderedDateTime = requestDto.OrderedDateTime!.Value;
        order.Note = requestDto.Note;
        order.CustomerId = requestDto.CustomerId;
        
        // Update order items.
        foreach (OrderItemRequestDto itemRequestDto in requestDto.Items)
        {
            OrderItem item;
            if (itemRequestDto.Id.HasValue)
            {
                item = order.Items.SingleOrDefault(i => i.Id == itemRequestDto.Id.Value);
                
                // Throw error if the item couldn't be found.
                if (item == null)
                {
                    string errorMessage = ErrorMessages.NotFound
                        .ReplaceResourceName(DisplayNames.OrderItem);
                    throw new OperationException(errorMessage);
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
        
        // Update photos.
        if (requestDto.Photos != null)
        {
            foreach (OrderPhotoRequestDto photoRequestDto in requestDto.Photos)
            {
                OrderPhoto photo;
                if (photoRequestDto.Id.HasValue)
                {
                    photo = order.Photos.SingleOrDefault(op => op.Id == photoRequestDto.Id);

                    if (photo == null)
                    {
                        string errorMessage = ErrorMessages.NotFound
                            .ReplaceResourceName(DisplayNames.Photo)
                            .ReplacePropertyName(DisplayNames.Id)
                            .ReplaceAttemptedValue(photoRequestDto.Id.ToString());
                        throw new OperationException($"photos[{photoRequestDto.Id}]", errorMessage);
                    }
                }
            }
        }
    }

    public async Task DeleteAsync(int id)
    {
        Order order = await _context.Orders
            .Where(o => !o.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Order),
                nameof(id),
                id.ToString());
        
        if (!_authorizationService.CanDeleteOrder(order))
        {
            throw new AuthorizationException();
        }

        try
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(sqlException);
            if (exceptionHandler.IsDeleteOrUpdateRestricted)
            {
                string errorMessage = ErrorMessages.DeleteRestricted
                    .ReplaceResourceName(DisplayNames.Order);
                throw new OperationException(nameof(id), errorMessage);
            }
            throw;
        }
    }
}