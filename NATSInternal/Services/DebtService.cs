namespace NATSInternal.Services;

/// <inheritdoc cref="IDebtService" />
public class DebtService : LockableEntityService, IDebtService
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
    public async Task<DebtDetailResponseDto> GetDetailAsync(int customerId, int debtId)
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
            .Where(d => d.CustomerId == customerId)
            .Where(d => d.Id == debtId)
            .Where(d => !d.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        return new DebtDetailResponseDto(
            debt,
            _authorizationService.GetDebtAuthorization(debt),
            mapUpdateHistories: shouldIncludeUpdateHistories);
    }
    
    /// <inheritdoc />
    public async Task<int> CreateAsync(int customerId, DebtUpsertRequestDto requestDto)
    {
        // Determining the incurred datetime.
        DateTime incurredDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.IncurredDateTime.HasValue)
        {
            // Check if the current user has permission to specify the created datetime for the debt.
            if (!_authorizationService.CanSetDebtIncurredDateTime())
            {
                throw new AuthorizationException();
            }

            incurredDateTime = requestDto.IncurredDateTime.Value;
        }
        
        // Initialize debt entity.
        Debt debt = new Debt
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            IncurredDateTime = incurredDateTime,
            CustomerId = customerId,
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
            HandleCreateOrUpdateException(sqlException, debt.CreatedUserId);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(int customerId, int debtId, DebtUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        Debt debt = await _context.Debts
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .Include(d => d.CreatedUser)
            .Where(d => d.CustomerId == customerId)
            .Where(d => d.Id == debtId)
            .Where(d => !d.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
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
            if (!_authorizationService.CanSetDebtIncurredDateTime())
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
                    if (debt.Customer.DebtRemainingAmount + amountDifference < 0)
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
                HandleCreateOrUpdateException(sqlException, debt.CreatedUserId);
            }
            
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(int customerId, int debtId)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        Debt debt = await _context.Debts
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .Where(d => d.CustomerId == customerId)
            .Where(d => d.Id == debtId)
            .Where(d => !d.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        // Ensure the user has permission to delete this debt.
        if (!_authorizationService.CanDeleteDebt())
        {
            throw new AuthorizationException();
        }
        
        //
        
        // Verify that if this debt is deleted, will the remaining debt amount be negative.
        if (debt.Customer.DebtRemainingAmount - debt.Amount < 0)
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
    /// <param name="userId">The user id of the debt.</param>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer with the specified id doesn't exist.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is any exception which is related to the data during the operation.
    /// </exception>
    private void HandleCreateOrUpdateException(
            MySqlException exception,
            int userId)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            switch (exceptionHandler.ViolatedFieldName)
            {
                case "customer_id":
                    throw new ResourceNotFoundException();
                default:
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceResourceName(DisplayNames.User)
                        .ReplaceAttemptedValue(userId.ToString());
                    throw new OperationException("id", errorMessage);
            }
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
