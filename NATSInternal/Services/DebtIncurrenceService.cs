namespace NATSInternal.Services;

/// <inheritdoc cref="IDebtIncurrenceService" />
public class DebtIncurrenceService :
        LockableEntityService,
        IDebtIncurrenceService
{
    private readonly DatabaseContext _context;
    private readonly IStatsService _statsService;
    private readonly IAuthorizationService _authorizationService;

    public DebtIncurrenceService(
            DatabaseContext context,
            IStatsService statsService,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _statsService = statsService;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<DebtIncurrenceDetailResponseDto> GetDetailAsync(
            int customerId,
            int debtId)
    {
        // Initialize query.
        IQueryable<DebtIncurrence> query = _context.DebtIncurrences
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
        DebtIncurrence debtIncurrence = await query
            .AsSplitQuery()
            .Where(d => d.CustomerId == customerId)
            .Where(d => d.Id == debtId)
            .Where(d => !d.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        return new DebtIncurrenceDetailResponseDto(
            debtIncurrence,
            _authorizationService.GetDebtAuthorization(debtIncurrence),
            mapUpdateHistories: shouldIncludeUpdateHistories);
    }
    
    /// <inheritdoc />
    public async Task<int> CreateAsync(
            int customerId,
            DebtIncurrenceUpsertRequestDto requestDto)
    {
        // Determining the incurred datetime.
        DateTime incurredDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.IncurredDateTime.HasValue)
        {
            // Check if the current user has permission to specify the
            // created datetime for the debt incurrence.
            if (!_authorizationService.CanSetDebtIncurredDateTime())
            {
                throw new AuthorizationException();
            }

            incurredDateTime = requestDto.IncurredDateTime.Value;
        }
        
        // Initialize debt incurrence entity.
        DebtIncurrence debtIncurrence = new DebtIncurrence
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            IncurredDateTime = incurredDateTime,
            CustomerId = customerId,
            CreatedUserId = _authorizationService.GetUserId()
        };
        _context.DebtIncurrences.Add(debtIncurrence);
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The debt is saved successfully, adjust the stats.
            await _statsService.IncrementDebtAmountAsync(
                debtIncurrence.Amount,
                DateOnly.FromDateTime(debtIncurrence.CreatedDateTime));
            
            // Commit the transaction, finish all operations.
            await transaction.CommitAsync();
            
            return debtIncurrence.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            HandleCreateOrUpdateException(sqlException);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(
            int customerId,
            int debtId,
            DebtIncurrenceUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        DebtIncurrence debt = await _context.DebtIncurrences
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
        DebtIncurrenceUpdateHistoryDataDto oldData;
        oldData = new DebtIncurrenceUpdateHistoryDataDto(debt);

        // Update the created datetime and amount if specified.
        if (requestDto.IncurredDateTime.HasValue)
        {
            // Check if the current user has permission to change the created datetime.
            if (!_authorizationService.CanSetDebtIncurredDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the consultant's IncurredDateTime to be modified when the
            // debt incurrence has already been locked.
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
                    if (debt.Customer.DebtAmount + amountDifference < 0)
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
        DebtIncurrenceUpdateHistoryDataDto newData;
        newData = new DebtIncurrenceUpdateHistoryDataDto(debt);
        
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
                HandleCreateOrUpdateException(sqlException);
            }
            
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(int customerId, int debtId)
    {
        // Fetch and ensure the entity with the given id exists in the database.
        DebtIncurrence debt = await _context.DebtIncurrences
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
        
        // Verify that if this debt is deleted, will the remaining debt amount be negative.
        if (debt.Customer.DebtAmount - debt.Amount < 0)
        {
            throw new OperationException(ErrorMessages.NegativeRemainingDebtAmount);
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Perform deleting operation and adjust stats.
        try
        {
            _context.DebtIncurrences.Remove(debt);
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
    /// Handles exception thrown by the database during the creating or updating operation.
    /// </summary>
    /// <param name="exception">
    /// An instance of the <see cref="MySqlException"/> class, containing the details of the
    /// error.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the customer with the specified id doesn't exist.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when the information of the requesting user has been deleted before the
    /// operation.
    /// </exception>
    private void HandleCreateOrUpdateException(MySqlException exception)
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
                    throw new ConcurrencyException();
            }
        }
    }
    
    /// <summary>
    /// Logs the old and new data to update history for the specified debt.
    /// </summary>
    /// <param name="debt">
    /// An instance of the <see cref="DebtIncurrence"/> entity class, representing the debt
    /// incurrence to which the log data belongs.
    /// </param>
    /// <param name="oldData">
    /// An instance of the <see cref="DebtIncurrenceUpdateHistoryDataDto"/> class, containing
    /// the old data of the debt before modification.
    /// </param>
    /// <param name="newData">
    /// An instance of the <see cref="DebtIncurrenceUpdateHistoryDataDto"/> class, containing
    /// the new data of the debt after modification. 
    /// </param>
    /// <param name="reason">
    /// A <see cref="string"/> value representing the reason of the update operation.
    /// </param>
    private void LogUpdateHistory(
            DebtIncurrence debt,
            DebtIncurrenceUpdateHistoryDataDto oldData,
            DebtIncurrenceUpdateHistoryDataDto newData,
            string reason)
    {
        DebtIncurrenceUpdateHistory updateHistory = new DebtIncurrenceUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId(),
            UpdatedDateTime = DateTime.UtcNow.ToApplicationTime()
        };
        
        debt.UpdateHistories ??= new List<DebtIncurrenceUpdateHistory>();
        debt.UpdateHistories.Add(updateHistory);
    }
}
