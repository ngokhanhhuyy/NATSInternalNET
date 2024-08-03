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
            case nameof(DebtListRequestDto.FieldOptions.IncurredDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.IncurredDateTime)
                        .ThenBy(d => d.Amount)
                    : query.OrderByDescending(d => d.IncurredDateTime)
                        .ThenByDescending(d => d.Amount);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(d => d.Amount)
                        .ThenBy(d => d.IncurredDateTime)
                    : query.OrderByDescending(d => d.Amount)
                        .ThenBy(d => d.IncurredDateTime);
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
        DebtListResponseDto responseDto = new DebtListResponseDto
        {
            Authorization = _authorizationService.GetDebtListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(d => new DebtBasicResponseDto(
                d,
                _authorizationService.GetDebtAuthorization(d)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        
        return responseDto;
    }
    
    /// <inheritdoc />
    public async Task<DebtDetailResponseDto> GetDetailAsync(int id)
    {
        // Initialize query.
        IQueryable<Debt> query = _context.Debts
            .Include(d => d.Customer)
            .Include(d => d.CreatedUser).ThenInclude(u => u.Roles);

        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessDebtUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(d => d.UpdateHistories);
        }

        // Fetch the entity with the given id and ensure it exists in the database.
        Debt debt = await query
            .AsSplitQuery()
            .SingleOrDefaultAsync(d => d.Id == id && !d.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Debt),
                nameof(id),
                id.ToString());
        
        return new DebtDetailResponseDto(
            debt,
            _authorizationService.GetDebtAuthorization(debt),
            mapUpdateHistories: shouldIncludeUpdateHistories);
}
    
    /// <inheritdoc />
    public async Task<int> CreateAsync(DebtUpsertRequestDto requestDto)
    {
        // Determining the created datetime.
        DateTime createdDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.IncurredDateTime.HasValue)
        {
            // Check if the current user has permission to specify the created datetime for the debt.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }

            createdDateTime = requestDto.IncurredDateTime.Value;
        }
        
        // Initialize debt entity.
        Debt debt = new Debt
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            CreatedDateTime = createdDateTime,
            CustomerId = requestDto.CustomerId,
            CreatedUserId = _authorizationService.GetUserId()
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
            HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debt.CreatedUserId);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(int id, DebtUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        Debt debt = await _context.Debts
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .Include(d => d.CreatedUser)
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
        DateOnly oldIncurredDate = DateOnly.FromDateTime(debt.IncurredDateTime);
        long oldAmount = debt.Amount;
        DebtUpdateHistoryDataDto oldData = new DebtUpdateHistoryDataDto(debt);

        // Update the created datetime and amount if specified.
        if (requestDto.IncurredDateTime.HasValue)
        {
            // Check if the current user has permission to change the created datetime.
            if (!_authorizationService.CanSetDebtCreatedDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the consultant's IncurredDateTime to be modified when the debt is locked.
            if (debt.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Debt)
                    .ReplacePropertyName(DisplayNames.IncurredDateTime);
                throw new OperationException(
                    nameof(requestDto.IncurredDateTime),
                    errorMessage);
            }
            
            // Assign the new PaidDateTime value only if it's different from the old one.
            if (requestDto.IncurredDateTime.Value != debt.IncurredDateTime)
            {
                // Verify if the amount has been changed, and with the new amount,
                // the remaning debt amount won't be negative.
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
                
                // Validate the IncurredDateTime from the request.
                try
                {
                    _statsService.ValidateStatsDateTime(
                        debt,
                        requestDto.IncurredDateTime.Value);
                }
                catch (ValidationException exception)
                {
                    string errorMessage = exception.Message
                        .ReplacePropertyName(DisplayNames.IncurredDateTime);
                    throw new OperationException(
                        nameof(requestDto.IncurredDateTime),
                        errorMessage);
                }
                
                // The specified IncurredDateTime is valid, assign it to the debt.
                debt.IncurredDateTime = requestDto.IncurredDateTime.Value;
            }
        }
        
        // Update other properties.
        debt.Amount = requestDto.Amount;
        debt.Note = requestDto.Note;
        
        // Store the new data for update history logging.
        DebtUpdateHistoryDataDto newData = new DebtUpdateHistoryDataDto(debt);
        
        // Log update history.
        LogUpdateHistory(debt, oldData, newData, requestDto.UpdatingReason);
        
        // Perform the update operations.
        try
        {
            await _context.SaveChangesAsync();
            
            // The debt can be saved successfully without any error.
            // Revert the stats for the old debt.
            await _statsService.IncrementDebtAmountAsync(- oldAmount, oldIncurredDate);

            // Add the stats for the changed debt.
            DateOnly newIncurredDate = DateOnly.FromDateTime(debt.IncurredDateTime);
            await _statsService.IncrementDebtAmountAsync(debt.Amount, newIncurredDate);
            
            // Commit the transaction, finish the operation.
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
                HandleCreateOrUpdateException(sqlException, requestDto.CustomerId, debt.CreatedUserId);
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
    private void HandleCreateOrUpdateException(
            MySqlException exception,
            int customerId,
            int userId)
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
    
    /// <summary>
    /// Log the old and new data to update history for the specified debt.
    /// </summary>
    /// <param name="debt">
    /// The debt entity which the new update history is associated.
    /// </param>
    /// <param name="oldData">
    /// An object containing the old data of the debt before modification.
    /// </param>
    /// <param name="newData">
    /// An object containing the new data of the debt after modification. 
    /// </param>
    /// <param name="reason">The reason of the modification.</param>
    private void LogUpdateHistory(
            Debt debt,
            DebtUpdateHistoryDataDto oldData,
            DebtUpdateHistoryDataDto newData,
            string reason)
    {
        DebtUpdateHistory updateHistory = new DebtUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId(),
            UpdatedDateTime = DateTime.UtcNow.ToApplicationTime()
        };
        
        debt.UpdateHistories ??= new List<DebtUpdateHistory>();
        debt.UpdateHistories.Add(updateHistory);
    }
}
