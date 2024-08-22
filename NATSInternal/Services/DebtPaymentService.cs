namespace NATSInternal.Services;

/// <inheritdoc cref="IDebtPaymentService" />
public class DebtPaymentService : LockableEntityService, IDebtPaymentService
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
    public async Task<DebtPaymentDetailResponseDto> GetDetailAsync(
            int customerId,
            int debtPaymentId)
    {
        // Initalize query.
        IQueryable<DebtPayment> query = _context.DebtPayments
            .Include(d => d.Customer)
            .Include(d => d.CreatedUser).ThenInclude(u => u.Role);
        
        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessDebtPaymentUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(dp => dp.UpdateHistories);
        }

        // Fetch the entity with the given id and ensure it exists in the database.
        DebtPayment debtPayment = await query
            .Where(dp => dp.CustomerId == customerId)
            .Where(dp => dp.Id == debtPaymentId)
            .Where(dp => !dp.IsDeleted)
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        return new DebtPaymentDetailResponseDto(
                debtPayment,
                _authorizationService.GetDebtPaymentAuthorization(debtPayment),
                mapUpdateHistories: shouldIncludeUpdateHistories);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(int customerId, DebtPaymentUpsertRequestDto requestDto)
    {
        // Determining the paid datetime.
        DateTime paidDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the created datetime
            // for the debt payment.
            if (!_authorizationService.CanSetDebtPaymentPaidDateTime())
            {
                throw new AuthorizationException();
            }
            
            paidDateTime = requestDto.PaidDateTime.Value;
        }

        // Verify that with the specified amount, the customer's remaining debt amount will
        // not be negative.
        string customerNotFoundErrorMessage = ErrorMessages.NotFoundByProperty
            .ReplaceResourceName(DisplayNames.Customer)
            .ReplacePropertyName(DisplayNames.Id)
            .ReplaceAttemptedValue(customerId.ToString());
        Customer customer = await _context.Customers
            .Include(c => c.DebtIncurrences)
            .Include(c => c.DebtPayments)
            .SingleOrDefaultAsync(c => c.Id == customerId)
            ?? throw new OperationException(nameof(customerId), customerNotFoundErrorMessage);
        if (customer.DebtAmount - requestDto.Amount < 0)
        {
            const string amountErrorMessage = ErrorMessages.NegativeRemainingDebtAmount;
            throw new OperationException(nameof(requestDto.Amount), amountErrorMessage);
        }
        
        // Initialize debt payment entity.
        DebtPayment debtPayment = new DebtPayment
        {
            Amount = requestDto.Amount,
            Note = requestDto.Note,
            PaidDateTime = paidDateTime,
            CustomerId = customerId,
            CreatedUserId = _authorizationService.GetUserId()
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
            HandleCreateOrUpdateException(sqlException, debtPayment.CreatedUserId);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(
            int customerId,
            int debtPaymentId,
            DebtPaymentUpsertRequestDto requestDto)
    {
        // Fetch and ensure the entity with the given debtPaymentId exists in the database.
        DebtPayment debtPayment = await _context.DebtPayments
            .Include(d => d.Customer).ThenInclude(c => c.DebtIncurrences)
            .Include(d => d.CreatedUser)
            .Where(dp => dp.CustomerId == customerId)
            .Where(dp => dp.Id == debtPaymentId)
            .Where(dp => !dp.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        // Check if the current user has permission to edit the debt payment.
        if (!_authorizationService.CanEditDebtPayment(debtPayment))
        {
            throw new AuthorizationException();
        }
        
        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Store the old data and create new data for stats adjustment.
        long oldAmount = debtPayment.Amount;
        DateOnly oldPaidDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
        DebtPaymentUpdateHistoryDataDto oldData;
        oldData = new DebtPaymentUpdateHistoryDataDto(debtPayment);

        // Update the paid datetime if specified.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to change the created datetime.
            if (!_authorizationService.CanSetDebtIncurredDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the consultant's PaidDateTime to be modified when the consultant is locked.
            if (debtPayment.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.DebtPayment)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(
                    nameof(requestDto.PaidDateTime),
                    errorMessage);
            }
            
            // Assign the new PaidDateTime value only if it's different from the old one.
            if (requestDto.PaidDateTime.Value != debtPayment.PaidDateTime)
            {
                // Verify if the amount has been changed, and with the new amount,
                // the remaning debt amount won't be negative.
                if (requestDto.Amount != debtPayment.Amount)
                {
                    long amountDifference = requestDto.Amount - debtPayment.Amount;
                    if (debtPayment.Customer.DebtAmount + amountDifference < 0)
                    {
                        throw new OperationException(
                            nameof(requestDto.Amount),
                            ErrorMessages.NegativeRemainingDebtAmount);
                    }
                }
                
                // Validate the specified PaidDateTime from the request.
                try
                {
                    _statsService.ValidateStatsDateTime(
                        debtPayment,
                        requestDto.PaidDateTime.Value);
                }
                catch (ValidationException exception)
                {
                    string errorMessage = exception.Message
                        .ReplacePropertyName(DisplayNames.PaidDateTime);
                    throw new OperationException(
                        nameof(requestDto.PaidDateTime),
                        errorMessage);
                }

                // The specified PaidDateTime is valid, assign it to the debt payment.
                debtPayment.PaidDateTime = requestDto.PaidDateTime.Value;
            }
        }

        // Verify that with the new paid amount, the customer's remaining debt amount will
        // not be negative.
        if (debtPayment.Customer.DebtAmount - requestDto.Amount < 0)
        {
            const string amountErrorMessage = ErrorMessages.NegativeRemainingDebtAmount;
            throw new OperationException(nameof(requestDto.Amount), amountErrorMessage);
        }
        
        // Update other properties.
        debtPayment.Amount = requestDto.Amount;
        debtPayment.Note = requestDto.Note;
        
        // Store new data for update history logging.
        DebtPaymentUpdateHistoryDataDto newData;
        newData = new DebtPaymentUpdateHistoryDataDto(debtPayment);
        
        // Log update history.
        LogUpdateHistory(debtPayment, oldData, newData, requestDto.UpdatingReason);
        
        // Perform the update operations.
        try
        {
            await _context.SaveChangesAsync();
            
            // The debt payment can be updated successfully without any error.
            // Adjust the stats.
            // Revert the old stats.
            await _statsService.IncrementDebtAmountAsync(-oldAmount, oldPaidDate);
            
            // Add new stats.
            DateOnly newPaidDate = DateOnly.FromDateTime(debtPayment.PaidDateTime);
            await _statsService.IncrementDebtAmountAsync(debtPayment.Amount, newPaidDate);
            
            // Commit the transaction and finish the operation.
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
                HandleCreateOrUpdateException(sqlException, debtPayment.CreatedUserId);
            }
            
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(int customerId, int debtPaymentId)
    {
        // Fetch and ensure the entity with the given debtPaymentId exists in the database.
        DebtPayment debtPayment = await _context.DebtPayments
            .Include(d => d.Customer).ThenInclude(c => c.DebtPayments)
            .Where(dp => dp.CustomerId == customerId)
            .Where(dp => dp.Id == debtPaymentId)
            .Where(dp => !dp.IsDeleted)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        // Ensure the user has permission to delete this debt payment.
        if (!_authorizationService.CanDeleteDebtPayment())
        {
            throw new AuthorizationException();
        }

        // Verify that if this debt payment is closed.
        if (debtPayment.IsLocked)
        {
            string errorMessage = ErrorMessages.ModificationTimeExpired
                .ReplaceResourceName(DisplayNames.DebtPayment);
            throw new OperationException(errorMessage);
        }
        
        // Verify that if this debt payment is deleted, will the remaining debt amount be negative.
        if (debtPayment.Customer.DebtAmount - debtPayment.Amount < 0)
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
    private void HandleCreateOrUpdateException(MySqlException exception, int userId)
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
                        .ReplacePropertyName(DisplayNames.Id);
                    errorMessage = errorMessage
                        .ReplaceResourceName(DisplayNames.User)
                        .ReplaceAttemptedValue(userId.ToString());
                    throw new OperationException("id", errorMessage);
            }
        }
    }
    
    /// <summary>
    /// Log the old and new data to update history for the specified debt payment.
    /// </summary>
    /// <param name="debtPayment">
    /// The debt payment entity which the new update history is associated.
    /// </param>
    /// <param name="oldData">
    /// An object containing the old data of the debt payment before modification.
    /// </param>
    /// <param name="newData">
    /// An object containing the new data of the debt payment after modification. 
    /// </param>
    /// <param name="reason">The reason of the modification.</param>
    private void LogUpdateHistory(
            DebtPayment debtPayment,
            DebtPaymentUpdateHistoryDataDto oldData,
            DebtPaymentUpdateHistoryDataDto newData,
            string reason)
    {
        DebtPaymentUpdateHistory updateHistory = new DebtPaymentUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UpdatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            UserId = _authorizationService.GetUserId()
        };
        debtPayment.UpdateHistories ??= new List<DebtPaymentUpdateHistory>();
        debtPayment.UpdateHistories.Add(updateHistory);
    }
}