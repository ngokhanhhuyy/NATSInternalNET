namespace NATSInternal.Services;

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

        // Initialize order payment.
        OrderPayment payment = await InitializeAsync(order, requestDto);
        return new OrderPaymentCreateResponseDto
        {
            OrderId = order.Id,
            OrderPaymentId = payment.Id
        };
    }

    public async Task<OrderPayment> CreateAsync(Order order, OrderPaymentRequestDto requestDto)
    {
        return await InitializeAsync(order, requestDto);
    }

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
            string errorMessage = ErrorMessages.ModificationTimeExpired.ReplaceResourceName(DisplayNames.OrderPayment);
            throw new OperationException(errorMessage);
        }

        // Begin transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        _context.OrderPayments.Remove(payment);
        try
        {
            await _context.SaveChangesAsync();
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

    private async Task<OrderPayment> InitializeAsync(Order order, OrderPaymentRequestDto requestDto)
    {
        // Check if the user has permission to modify the order.
        if (!_authorizationService.CanEditOrder(order))
        {
            throw new AuthorizationException();
        }

        // Check if order's payments have been completed.
        if (order.Dept <= 0)
        {
            string errorMessage = ErrorMessages.PaymentAlreadyCompleted
                .ReplaceResourceName(DisplayNames.Order);
            throw new OperationException(errorMessage);
        }

        // Initialize payment entity.
        OrderPayment payment = new OrderPayment
        {
            Amount = requestDto.Amount,
            PaidDateTime = requestDto.PaidDateTime ?? DateTime.UtcNow.ToApplicationTime(),
            Note = requestDto.Note,
            UserId = _authorizationService.GetUserId()
        };

        if (order.Payments == null)
        {
            order.Payments = new List<OrderPayment>();
        }
        order.Payments.Add(payment);

        try
        {
            await _context.SaveChangesAsync();
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
                throw new OperationException(errorMessage);
            }
            throw;
        }
    }
}
