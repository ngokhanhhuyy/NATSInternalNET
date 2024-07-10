namespace NATSInternal.Services;

/// <inheritdoc />
public class OrderPaymentService : IOrderPaymentService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;

    public OrderPaymentService(
            DatabaseContext context,
            IAuthorizationService authorizationService,
            IStatsService statsService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<OrderPaymentResponseDto> GetDetailAsync(int id)
    {
        return await _context.OrderPayments
            .Include(op => op.User)
            .Where(op => op.Id == id)
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
            }).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(nameof(OrderPayment), nameof(id), id.ToString());
    }

    /// <inheritdoc />
    public async Task<OrderPaymentCreateResponseDto> CreateAsync(
            int orderId,
            OrderPaymentRequestDto requestDto)
    {
        // Fetch order entity from the database and ensure it exists.
        Order order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .SingleOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId)
            ?? throw new ResourceNotFoundException(
                nameof(Order),
                DisplayNames.Id,
                orderId.ToString());

        // Check if the user has permission to modify the order.
        if (!_authorizationService.CanEditOrder(order))
        {
            throw new AuthorizationException();
        }

        // Initialize order payment.
        OrderPayment payment = await PerformCreatingOperationAsync(order, requestDto);
        return new OrderPaymentCreateResponseDto
        {
            OrderId = order.Id,
            OrderPaymentId = payment.Id
        };
    }

    /// <inheritdoc />
    public async Task<OrderPayment> CreateAsync(
            Order order,
            OrderPaymentRequestDto requestDto,
            string propertyPathPrefix)
    {
        return await PerformCreatingOperationAsync(order, requestDto, propertyPathPrefix);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, OrderPaymentRequestDto requestDto)
    {
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Fetch the entity from the dâtbase and ensure it exists.
        OrderPayment payment = await _context.OrderPayments
            .Include(p => p.Order)
            .SingleOrDefaultAsync(p => !p.Order.IsDeleted && p.Id == id)
            ?? throw new ResourceNotFoundException(nameof(OrderPayment), nameof(id), id.ToString());

        // Ensure the order hasn't been closed.
        if (payment.IsClosed)
        {
            string errorMessage = ErrorMessages.ModificationTimeExpired.ReplaceResourceName(DisplayNames.OrderPayment);
            throw new OperationException(errorMessage);
        }

        // Remove stats of the payment's previous logged datetime.
        await _statsService.IncrementRetailRevenueAsync(-payment.Amount, DateOnly.FromDateTime(payment.PaidDateTime));

        // Update payment properties.
        payment.Amount = requestDto.Amount;
        payment.PaidDateTime = requestDto.PaidDateTime!.Value;
        payment.Note = requestDto.Note;

        // Add new stats for the payment's currently logged datetime.
        await _statsService.IncrementRetailRevenueAsync(payment.Amount, DateOnly.FromDateTime(payment.PaidDateTime));

        // Save changes and commits.
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure it exists.
        OrderPayment payment = await _context.OrderPayments
            .Include(p => p.Order)
            .SingleOrDefaultAsync(p => !p.Order.IsDeleted && p.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(OrderPayment),
                nameof(id),
                id.ToString());

        // Check if the payment or the order which the payment belongs to has been closed.
        if (payment.IsClosed || payment.Order.IsClosed)
        {
            string errorMessage = ErrorMessages.ModificationTimeExpired
                .ReplaceResourceName(DisplayNames.OrderPayment);
            throw new OperationException(errorMessage);
        }

        // Begin transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        _context.OrderPayments.Remove(payment);
        try
        {
            await _context.SaveChangesAsync();

            // No error occurs, adjust stats.
            await _statsService.IncrementRetailRevenueAsync(
                -payment.Amount,
                DateOnly.FromDateTime(payment.PaidDateTime));     
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(sqlException);
            if (exceptionHandler.IsDeleteOrUpdateRestricted)
            {
                string errorMessage = ErrorMessages.DeleteRestricted
                    .ReplaceResourceName(DisplayNames.OrderPayment);
                throw new OperationException(errorMessage);
            }
            throw;
        }
    }

    /// <summary>
    /// Perform the operation which creates a new payment associated to the given order.
    /// </summary>
    /// <param name="order">The order which the payment to be created is associated with.</param>
    /// <param name="requestDto">An object containing the data for a new payment.</param>
    /// <param name="propertyPathPrefix">
    /// The prefix of the property paths if there is any exception thrown.
    /// The default value is an empty string.
    /// </param>
    /// <returns></returns>
    /// <exception cref="OperationException"></exception>
    private async Task<OrderPayment> PerformCreatingOperationAsync(
            Order order,
            OrderPaymentRequestDto requestDto,
            string propertyPathPrefix = "")
    {
        // Check if order's payments have been completed.
        if (order.Dept <= 0)
        {
            string errorMessage = ErrorMessages.PaymentAlreadyCompleted
                .ReplaceResourceName(DisplayNames.Order);
            throw new OperationException(propertyPathPrefix, errorMessage);
        }

        // Initialize payment entity.
        OrderPayment payment = new OrderPayment
        {
            Amount = requestDto.Amount,
            PaidDateTime = requestDto.PaidDateTime ?? DateTime.UtcNow.ToApplicationTime(),
            Note = requestDto.Note,
            UserId = _authorizationService.GetUserId()
        };

        // Initialize a list of payments for the order in order to add the payment later.
        if (order.Payments == null)
        {
            order.Payments = new List<OrderPayment>();
        }
        order.Payments.Add(payment);

        try
        {
            // Save changes.
            await _context.SaveChangesAsync();

            // No error occured, adjusting the stats.
            await _statsService.IncrementRetailRevenueAsync(
                payment.Amount,
                DateOnly.FromDateTime(payment.PaidDateTime));
            return payment;
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
                    case "order_id":
                        errorMessage = ErrorMessages.NotFound
                            .ReplaceResourceName(DisplayNames.Order);
                        break;
                    case "user_id":
                        errorMessage = ErrorMessages.NotFound
                            .ReplaceResourceName(DisplayNames.User);
                        break;
                    default:
                        errorMessage = ErrorMessages.Undefined;
                        break;
                }
                throw new OperationException(propertyPathPrefix, errorMessage);
            }
            throw;
        }
    }
}