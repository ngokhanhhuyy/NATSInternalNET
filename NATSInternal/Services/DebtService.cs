namespace NATSInternal.Services;

/// <inheritdoc />
public class DebtService : IDebtService
{
    private readonly DatabaseContext _context;
    private readonly IStatsService _statsService;
    private readonly IAuthorizationService _authorizationService;
    
    public DebtService(
            DatabaseContext context,
            IStatsService statsService,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _statsService = statsService;
        _authorizationService = authorizationService;
    }
    
    /// <inheritdoc />
    public async Task<DebtListResponseDto> GetListAsync(DebtListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<Debt> query = _context.Debts;
        
        // Filter by fields.
        switch (requestDto.OrderByField)
        {
            case nameof(DebtListRequestDto.FieldOptions.CreatedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.CreatedDateTime)
                        .ThenBy(d => d.Amount)
                    : query.OrderByDescending(d => d.CreatedDateTime)
                        .ThenByDescending(d => d.Amount);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.Amount)
                        .ThenBy(d => d.CreatedDateTime)
                    : query.OrderByDescending(d => d.Amount)
                        .ThenBy(d => d.CreatedDateTime);
                break;
        }
        
        // Filter by range from if specified in the request.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(d => d.CreatedDateTime >= rangeFromDateTime);
        }
        
        // Filter by range to if specified in the request.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value.AddDays(1), new TimeOnly(0, 0, 0));
            query = query.Where(d => d.CreatedDateTime < rangeToDateTime);
        }

        // Filter by not being soft deleted.
        query = query.Where(o => !o.IsDeleted);
        
        // Initialize repsonse dto.
        DebtListResponseDto responseDto = new DebtListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(d => new DebtBasicResponseDto
            {
                Id = d.Id,
                Amount = d.Amount,
                Note = d.Note,
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
                Authorization = _authorizationService.GetDebtAuthorization(d)
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        responseDto.Authorization = _authorizationService.GetDebtListAuthorization();
        
        return responseDto;
    }
    
    /// <inheritdoc />
    public async Task<DebtDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Debts
            .Include(d => d.Customer)
            .Include(d => d.User).ThenInclude(u => u.Role)
            .Where(d => d.Id == id && !d.IsDeleted)
            .Select(d => new DebtDetailResponseDto
            {
                Id = d.Id,
                Amount = d.Amount,
                Note = d.Note,
                CreatedDateTime = d.CreatedDateTime,
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
            }).SingleOrDefaultAsync();
    }
    
    /// <inheritdoc />
    public async Task<int> CreateAsync(DebtUpsertRequestDto requestDto)
    {
        // Determining the created datetime.
        DateTime createdDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.CreatedDateTime.HasValue)
        {
            // Check if the current user has permission to specify the created datetime for the debt.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }
            
            // Verify that with the specified created datetime, the debt will not be closed.
            if (!_statsService.VerifyResourceDateTimeToBeCreated(requestDto.CreatedDateTime.Value))
            {
                string errorMessage = ErrorMessages.GreaterThanOrEqual
                    .ReplacePropertyName(DisplayNames.CreatedDateTime)
                    .ReplaceComparisonValue(_statsService.GetResourceMinimumOpenedDateTime().ToVietnameseString());
                throw new OperationException(nameof(requestDto.CreatedDateTime), errorMessage);
            }
            
            // The specified created datetime is valid, assign it to the debt.
            createdDateTime = requestDto.CreatedDateTime.Value;
        }
        
        // Initialize debt entity.
        Debt debt = new Debt
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            CreatedDateTime = createdDateTime,
            CustomerId = requestDto.CustomerId,
            UserId = _authorizationService.GetUserId()
        };
        _context.Debts.Add(debt);
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The debt is saved successfully, adjust the stats.
            await _statsService.IncrementDebtAmountAsync(
                debt.Amount,
                DateOnly.FromDateTime(debt.CreatedDateTime));
            
            // Commit the transaction, finish all operations.
            await transaction.CommitAsync();
            
            return debt.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debt.UserId);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(int id, DebtUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        Debt debt = await _context.Debts
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .Include(d => d.User)
            .SingleOrDefaultAsync(d => d.Id == id && !d.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(Debt), nameof(id), id.ToString());
        
        // Check if the current user has permission to edit the debt.
        if (!_authorizationService.CanEditDebt(debt))
        {
            throw new AuthorizationException();
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Store the old data and create new data for stats adjustment.
        DateOnly oldCreatedDate = DateOnly.FromDateTime(debt.CreatedDateTime);
        long oldAmount = debt.Amount;

        // Update the created datetime and amount if specified.
        if (requestDto.CreatedDateTime.HasValue)
        {
            // Check if the current user has permission to change the created datetime.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }
            
            // Check if the amount or created datetime has been actually changed.
            DateOnly createdDateFromRequest = DateOnly.FromDateTime(requestDto.CreatedDateTime.Value);
            if (createdDateFromRequest != oldCreatedDate || requestDto.Amount != debt.Amount)
            {
            
                // Verify if the amount has been changed, and with the new amount, the remaning debt amount
                // won't be negative.
                if (requestDto.Amount != debt.Amount)
                {
                    long amountDifference = requestDto.Amount - debt.Amount;
                    if (debt.Customer.RemainingDebtAmount + amountDifference < 0)
                    {
                        throw new OperationException(
                            nameof(requestDto.Amount),
                            ErrorMessages.NegativeRemainingDebtAmount);
                    }
                }
                
                // Check if that with the new created datetime, the status of the debt won't be changed.
                bool willDebtStatusNotBeChanged = _statsService
                    .VerifyResourceDateTimeToBeUpdated(debt.CreatedDateTime, requestDto.CreatedDateTime.Value);
                if (!willDebtStatusNotBeChanged)
                {
                    DateTime minAllowedDateTime = _statsService.GetResourceMinimumOpenedDateTime();
                    string errorMessage = ErrorMessages.GreaterThanOrEqual
                        .ReplacePropertyName(DisplayNames.CreatedDateTime)
                        .ReplaceComparisonValue(minAllowedDateTime.ToVietnameseString());
                    throw new OperationException(nameof(requestDto.CreatedDateTime), errorMessage);
                }
                
                // Change the created datetime.
                debt.Amount = requestDto.Amount;
                debt.CreatedDateTime = requestDto.CreatedDateTime.Value;
            }
        }
        
        // Update other properties.
        debt.Amount = requestDto.Amount;
        debt.Note = requestDto.Note;
        debt.CustomerId = requestDto.CustomerId;
                
        // Undo the stats for the old debt.
        await _statsService.IncrementDebtAmountAsync(- oldAmount, oldCreatedDate);

        // Readjust the stats for the changed debt.
        DateOnly newCreatedDate = DateOnly.FromDateTime(debt.CreatedDateTime);
        await _statsService.IncrementDebtAmountAsync(debt.Amount, newCreatedDate);
        
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
                HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debt.UserId);
            }
            
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        Debt debt = await _context.Debts
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .SingleOrDefaultAsync(d => d.Id == id && !d.IsDeleted)
            ?? throw new ResourceNotFoundException(nameof(Debt), nameof(id), id.ToString());
        
        // Verify that if this debt is deleted, will the remaining debt amount be negative.
        if (debt.Customer.RemainingDebtAmount - debt.Amount < 0)
        {
            throw new OperationException(ErrorMessages.NegativeRemainingDebtAmount);
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform deleting operation and adjust stats.
        try
        {
            _context.Debts.Remove(debt);
            await _context.SaveChangesAsync();
            
            // Debt has been deleted successfully, adjust the stats.
            DateOnly createdDate = DateOnly.FromDateTime(debt.CreatedDateTime);
            await _statsService.IncrementDebtAmountAsync(- debt.Amount, createdDate);
            
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
                    debt.IsDeleted = true;
                    
                    // Adjust the stats.
                    DateOnly createdDate = DateOnly.FromDateTime(debt.CreatedDateTime);
                    await _statsService.IncrementDebtAmountAsync(debt.Amount, createdDate);
                    
                    // Save changes and commit the transaction again, finish the operation.
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
        }
    }
    
    /// <summary>
    /// Handle exception thrown by the database during the creating or updating operation.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    /// <param name="customerId">The customer id of the debt.</param>
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
}
