namespace NATSInternal.Services;

/// <inheritdoc />
public class DebtPaymentService : IDebtPaymentService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;

    public DebtPaymentService(
            DatabaseContext context,
            IAuthorizationService authorizationService,
            IStatsService statsService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<DebtPaymentListResponseDto> GetListAsync(
            DebtPaymentListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<DebtPayment> query = _context.DebtPayments;
        
        // Filter by fields.
        switch (requestDto.OrderByField)
        {
            case nameof(DebtListRequestDto.FieldOptions.CreatedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.PaidDateTime)
                        .ThenBy(d => d.Amount)
                    : query.OrderByDescending(d => d.PaidDateTime)
                        .ThenByDescending(d => d.Amount);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.Amount)
                        .ThenBy(d => d.PaidDateTime)
                    : query.OrderByDescending(d => d.Amount)
                        .ThenBy(d => d.PaidDateTime);
                break;
        }
        
        // Filter by range from if specified in the request.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(d => d.PaidDateTime >= rangeFromDateTime);
        }
        
        // Filter by range to if specified in the request.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value.AddDays(1), new TimeOnly(0, 0, 0));
            query = query.Where(d => d.PaidDateTime < rangeToDateTime);
        }

        // Filter by not being soft deleted.
        query = query.Where(o => !o.IsDeleted);
        
        // Initialize repsonse dto.
        DebtPaymentListResponseDto responseDto = new DebtPaymentListResponseDto
        {
            Authorization = _authorizationService.GetDebtPaymentListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(dp => new DebtPaymentBasicResponseDto
            {
                Id = dp.Id,
                Amount = dp.Amount,
                Note = dp.Note,
                IsClosed = dp.IsClosed,
                Customer = new CustomerBasicResponseDto
                {
                    Id = dp.Customer.Id,
                    FullName = dp.Customer.FullName,
                    NickName = dp.Customer.NickName,
                    Gender = dp.Customer.Gender,
                    Birthday = dp.Customer.Birthday,
                    PhoneNumber = dp.Customer.PhoneNumber
                },
                Authorization = _authorizationService.GetDebtPaymentAuthorization(dp)
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        
        return responseDto;
    }

    /// <inheritdoc />
    public async Task<DebtPaymentDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.DebtPayments
            .Include(d => d.Customer)
            .Include(d => d.User).ThenInclude(u => u.Role)
            .Where(d => d.Id == id && !d.IsDeleted)
            .Select(d => new DebtPaymentDetailResponseDto
            {
                Id = d.Id,
                Amount = d.Amount,
                Note = d.Note,
                PaidDateTime = d.PaidDateTime,
                IsClosed = d.IsClosed,
                Customer = new CustomerBasicResponseDto
                {
                    Id = d.Customer.Id,
                    FullName = d.Customer.FullName,
                    NickName = d.Customer.NickName,
                    Gender = d.Customer.Gender,
                    Birthday = d.Customer.Birthday,
                    PhoneNumber = d.Customer.PhoneNumber
                },
                User = new UserBasicResponseDto
                {
                    Id = d.User.Id,
                    UserName = d.User.UserName,
                    FirstName = d.User.FirstName,
                    MiddleName = d.User.MiddleName,
                    LastName = d.User.LastName,
                    FullName = d.User.FullName,
                    Gender = d.User.Gender,
                    Birthday = d.User.Birthday,
                    JoiningDate = d.User.JoiningDate,
                    AvatarUrl = d.User.AvatarUrl,
                    Role = new RoleBasicResponseDto
                    {
                        Id = d.User.Role.Id,
                        Name = d.User.Role.Name,
                        DisplayName = d.User.Role.DisplayName,
                        PowerLevel = d.User.Role.PowerLevel
                    }
                }
            }).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(DebtPayment),
                nameof(id),
                id.ToString());;
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(DebtPaymentUpsertRequestDto requestDto)
    {
        // Determining the paid datetime.
        DateTime paidDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the created datetime
            // for the debt payment.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }
            
            // Verify that with the specified paid datetime, the debt payment will not be closed.
            if (!_statsService.VerifyResourceDateTimeToBeCreated(requestDto.PaidDateTime.Value))
            {
                string errorMessage = ErrorMessages.GreaterThanOrEqual
                    .ReplacePropertyName(DisplayNames.CreatedDateTime)
                    .ReplaceComparisonValue(_statsService
                        .GetResourceMinimumOpenedDateTime()
                        .ToVietnameseString());
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }
            
            // The specified paid datetime is valid, assign it to the debt payment.
            paidDateTime = requestDto.PaidDateTime.Value;
        }

        // Verify that with the specified amount, the customer's remaining debt amount will
        // not be negative.
        string customerNotFoundErrorMessage = ErrorMessages.NotFoundByProperty
            .ReplaceResourceName(DisplayNames.Customer)
            .ReplacePropertyName(DisplayNames.Id)
            .ReplaceAttemptedValue(requestDto.CustomerId.ToString());
        Customer customer = await _context.Customers
            .Include(c => c.Debts)
            .Include(c => c.DebtPayments)
            .SingleOrDefaultAsync(c => c.Id == requestDto.CustomerId)
            ?? throw new OperationException(nameof(requestDto.CustomerId), customerNotFoundErrorMessage);
        if (customer.RemainingDebtAmount - requestDto.Amount < 0)
        {
            string amountErrorMessage = ErrorMessages.NegativeRemainingDebtAmount;
            throw new OperationException(nameof(requestDto.Amount), amountErrorMessage);
        }
        
        // Initialize debt payment entity.
        DebtPayment debtPayment = new DebtPayment
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            PaidDateTime = paidDateTime,
            CustomerId = requestDto.CustomerId,
            UserId = _authorizationService.GetUserId()
        };
        _context.DebtPayments.Add(debtPayment);
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The debt is saved successfully, adjust the stats.
            await _statsService.IncrementDebtPaidAmountAsync(
                debtPayment.Amount,
                DateOnly.FromDateTime(debtPayment.PaidDateTime));
            
            // Commit the transaction, finish all operations.
            await transaction.CommitAsync();
            
            return debtPayment.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debtPayment.UserId);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(int id, DebtPaymentUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        DebtPayment debtPayment = await _context.DebtPayments
            .Include(d => d.Customer).ThenInclude(c => c.Debts)
            .Include(d => d.User)
            .SingleOrDefaultAsync(d => d.Id == id && !d.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(Debt), nameof(id), id.ToString());
        
        // Check if the current user has permission to edit the debt payment.
        if (!_authorizationService.CanEditDebtPayment(debtPayment))
        {
            throw new AuthorizationException();
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Store the old data and create new data for stats adjustment.
        DateOnly oldPaidDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
        long oldAmount = debtPayment.Amount;

        // Update the paid datetime if specified.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to change the created datetime.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }
            
            // Check if the amount or created datetime has been actually changed.
            DateOnly createdDateFromRequest = DateOnly.FromDateTime(requestDto.PaidDateTime.Value);
            if (createdDateFromRequest != oldPaidDate || requestDto.Amount != debtPayment.Amount)
            {
            
                // Verify if the amount has been changed, and with the new amount,
                // the remaning debt amount won't be negative.
                if (requestDto.Amount != debtPayment.Amount)
                {
                    long amountDifference = requestDto.Amount - debtPayment.Amount;
                    if (debtPayment.Customer.RemainingDebtAmount + amountDifference < 0)
                    {
                        throw new OperationException(
                            nameof(requestDto.Amount),
                            ErrorMessages.NegativeRemainingDebtAmount);
                    }
                }
                
                // Check if that with the new paid datetime, the status of the debt won't be changed.
                bool willDebtStatusNotBeChanged = _statsService.VerifyResourceDateTimeToBeUpdated(
                        debtPayment.PaidDateTime,
                        requestDto.PaidDateTime.Value);
                if (!willDebtStatusNotBeChanged)
                {
                    DateTime minAllowedDateTime = _statsService.GetResourceMinimumOpenedDateTime();
                    string errorMessage = ErrorMessages.GreaterThanOrEqual
                        .ReplacePropertyName(DisplayNames.PaidDateTime)
                        .ReplaceComparisonValue(minAllowedDateTime.ToVietnameseString());
                    throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
                }
                
                // Change the paid datetime.
                debtPayment.PaidDateTime = requestDto.PaidDateTime.Value;
            }
        }

        // Verify that with the new paid amount, the customer's remaining debt amount will
        // not be negative.
        if (debtPayment.Customer.RemainingDebtAmount - requestDto.Amount < 0)
        {
            string amountErrorMessage = ErrorMessages.NegativeRemainingDebtAmount;
            throw new OperationException(nameof(requestDto.Amount), amountErrorMessage);
        }
        
        // Update other properties.
        debtPayment.Amount = requestDto.Amount;
        debtPayment.Note = requestDto.Note;
                
        // Undo the stats for the old debt.
        await _statsService.IncrementDebtPaidAmountAsync(- oldAmount, oldPaidDate);

        // Readjust the stats for the changed debt amount.
        DateOnly newPaidDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
        await _statsService.IncrementDebtPaidAmountAsync(debtPayment.Amount, newPaidDate);
        
        // Perform the update operations.
        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        {
            // Handling concurrecy exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handling data exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debtPayment.UserId);
            }
            
            throw;
        }
    }

    /// <summary>
    /// Handle exception thrown by the database during the creating or updating operation.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    /// <param name="customerId">The customer id of the debt payment.</param>
    /// <param name="userId">The user id of the debt.</param>
    /// <exception cref="OperationException">
    /// Thrown when there is any exception which is related to the data during the operation.
    /// </exception>
    private void HandleCreateOrUpdateException(MySqlException exception, int customerId, int userId)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            string errorMessage = ErrorMessages.NotFoundByProperty
                .ReplacePropertyName(DisplayNames.Id);
            switch (exceptionHandler.ViolatedFieldName)
            {
                case "customer_id":
                    errorMessage = errorMessage
                        .ReplaceResourceName(DisplayNames.Customer)
                        .ReplaceAttemptedValue(customerId.ToString());
                    break;
                default:
                    errorMessage = errorMessage
                        .ReplaceResourceName(DisplayNames.User)
                        .ReplaceAttemptedValue(userId.ToString());
                    break;
            }
            throw new OperationException("id", errorMessage);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        DebtPayment debtPayment = await _context.DebtPayments
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .SingleOrDefaultAsync(d => d.Id == id && !d.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(Debt), nameof(id), id.ToString());

        // Verify that if this debt payment is closed.
        if (debtPayment.IsClosed)
        {
            string errorMessage = ErrorMessages.ModificationTimeExpired
                .ReplaceResourceName(DisplayNames.DebtPayment);
            throw new OperationException(errorMessage);
        }
        
        // Verify that if this debt payment is deleted, will the remaining debt amount be negative.
        if (debtPayment.Customer.RemainingDebtAmount - debtPayment.Amount < 0)
        {
            throw new OperationException(ErrorMessages.NegativeRemainingDebtAmount);
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform deleting operation and adjust stats.
        try
        {
            _context.DebtPayments.Remove(debtPayment);
            await _context.SaveChangesAsync();
            
            // Debt payment has been deleted successfully, adjust the stats.
            DateOnly createdDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
            await _statsService.IncrementDebtPaidAmountAsync(- debtPayment.Amount, createdDate);
            
            // Commit the transaction, finish the operation.
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        {
            // Handle concurrency exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handle deleting restricted exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                // Soft delete when the entity is restricted to be deleted.
                if (exceptionHandler.IsDeleteOrUpdateRestricted)
                {
                    debtPayment.IsDeleted = true;
                    
                    // Adjust the stats.
                    DateOnly createdDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
                    await _statsService.IncrementDebtAmountAsync(debtPayment.Amount, createdDate);
                    
                    // Save changes and commit the transaction again, finish the operation.
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
        }
    }
}