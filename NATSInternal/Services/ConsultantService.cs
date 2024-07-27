namespace NATSInternal.Services;

/// <inheritdoc />
public class ConsultantService : IConsultantService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;

    public ConsultantService(
            DatabaseContext context,
            IAuthorizationService authorizationService,
            IStatsService statsService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<ConsultantListResponseDto> GetListAsync(
            ConsultantListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<Consultant> query = _context.Consultants;
        
        // Sorting direction and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(ConsultantListRequestDto.FieldOptions.Amount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(e => e.Amount)
                        .ThenBy(e => e.PaidDateTime)
                    : query.OrderByDescending(e => e.Amount)
                        .ThenByDescending(e => e.PaidDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(e => e.PaidDateTime)
                        .ThenBy(e => e.Amount)
                    : query.OrderByDescending(e => e.PaidDateTime)
                        .ThenByDescending(e => e.Amount);
                break;
        }

        // Filter from range if specified.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.PaidDateTime >= rangeFromDateTime);
        }

        // Filter to range if specified.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.PaidDateTime <= rangeToDateTime);
        }
        
        // Initialize response dto.
        ConsultantListResponseDto responseDto = new ConsultantListResponseDto
        {
            Authorization = _authorizationService.GetConsultantListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(c => new ConsultantBasicResponseDto(
                c,
                _authorizationService.GetConsultantAuthorization(c)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();
        
        return responseDto;
    }

    /// <inheritdoc />
    public async Task<ConsultantDetailResponseDto> GetDetailAsync(int id)
    {
        Consultant consultant = await _context.Consultants
            .Include(c => c.CreatedUser).ThenInclude(c => c.Roles)
            .Include(c => c.Customer)
            .Include(c => c.UpdateHistories)
            .Where(e => e.Id == id)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Expense),
                nameof(id),
                id.ToString());
        
        return new ConsultantDetailResponseDto(
            consultant,
            _authorizationService.GetConsultantAuthorization(consultant),
            mapUpdateHistory: _authorizationService.CanAccessConsultantUpdateHistories());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(ConsultantUpsertRequestDto requestDto)
    {
        // Use transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Determine paid datetime.
        DateTime paidDateTime = DateTime.UtcNow.ToApplicationTime();
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the paid datetime.
            if (!_authorizationService.CanSetExpensePaidDateTime())
            {
                throw new AuthorizationException();
            }

            // Verify that with the specified paid datetime, the expense will not be
            // considered as closed.
            if (!_statsService.VerifyResourceDateTimeToBeCreated(requestDto.PaidDateTime.Value))
            {
                DateTime minimumAllowedDateTime = _statsService.GetResourceMinimumOpenedDateTime();
                string errorMessage = ErrorMessages.GreaterThanOrEqual
                    .ReplacePropertyName(DisplayNames.PaidDateTime)
                    .ReplaceComparisonValue(minimumAllowedDateTime.ToVietnameseString());
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }

            // The specified paid datetime is valid, assign the expense to it.
            paidDateTime = requestDto.PaidDateTime.Value;
        }
        
        // Initialize entity.
        Consultant consultant = new Consultant
        {
            Amount = requestDto.Amount,
            PaidDateTime = paidDateTime,
            Note = requestDto.Note,
            CustomerId = requestDto.CustomerId,
            CreatedUserId = _authorizationService.GetUserId()
        };
        _context.Consultants.Add(consultant);
        
        // Save changes and commit.
        try
        {
            await _context.SaveChangesAsync();

            // Consultant can be created successfully, adjust the stats.
            DateOnly paidDate = DateOnly.FromDateTime(paidDateTime);
            await _statsService.IncrementConsultantGrossRevenueAsync(consultant.Amount, paidDate);

            // Commit the transaction and finish the operation.
            await transaction.CommitAsync();
            return consultant.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            // Handle exception and convert to the appropriate exception.
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(sqlException);
            if (exceptionHandler.IsForeignKeyNotFound)
            {
                string propertyName = string.Empty;
                string errorMessage = ErrorMessages.NotFound;
                switch (exceptionHandler.ViolatedFieldName)
                {
                    case "user_id":
                        propertyName = nameof(consultant.CreatedUserId);
                        errorMessage = errorMessage.ReplaceResourceName(DisplayNames.User);
                        break;
                    case "customer_id":
                        propertyName = nameof(consultant.CustomerId);
                        errorMessage = errorMessage.ReplaceResourceName(DisplayNames.Customer);
                        break;
                }
                throw new OperationException(propertyName, errorMessage);
            }
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, ConsultantUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Consultant consultant = await _context.Consultants
            .Include(c => c.CreatedUser)
            .Include(c => c.Customer)
            .Where(c => c.Id == id && !c.IsDeleted)
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Expense),
                nameof(id),
                id.ToString());
        
        // Ensure the entity is editable by the requester.
        if (!_authorizationService.CanEditConsultant(consultant))
        {
            throw new AuthorizationException();
        }

        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        // Store the old data for history logging and stats adjustment.
        ConsultantUpdateHistoryDataDto oldData;
        oldData = new ConsultantUpdateHistoryDataDto(consultant);
        long oldAmount = consultant.Amount;
        DateOnly oldPaidDate = DateOnly.FromDateTime(consultant.PaidDateTime);

        // Determining the PaidDateTime value based on the specified data from the request.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the paid datetime.
            if (!_authorizationService.CanSetExpensePaidDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the consultant's PaidDateTime to be modified when the consultant is locked.
            if (consultant.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Consultant)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }

            // Validate the specfied PaidDateTime from the request.
            try
            {
                consultant.PaidDateTime = requestDto.PaidDateTime.Value;
            }
            catch (ArgumentException exception)
            {
                string errorMessage = exception.Message
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(nameof(requestDto.PaidDateTime), errorMessage);
            }
        }

        // Update fields.
        consultant.Amount = requestDto.Amount;
        consultant.Note = requestDto.Note;
        
        // Storing new data for update history logging.
        ConsultantUpdateHistoryDataDto newData;
        newData = new ConsultantUpdateHistoryDataDto(consultant);
        
        // Initialize update history.
        LogUpdateHistory(consultant, oldData, newData, requestDto.UpdatingReason);
        
        // Perform the updating operation.
        try
        {
            await _context.SaveChangesAsync();

            // Consultant can be updated without any error.
            // Revert the old stats.
            await _statsService.IncrementConsultantGrossRevenueAsync(
                - oldAmount,
                oldPaidDate);

            // Adjust new stats.
            await _statsService.IncrementConsultantGrossRevenueAsync(
                consultant.Amount,
                DateOnly.FromDateTime(consultant.PaidDateTime));

            // Commit the transaction and finishing the operation.
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            // Handle exception and convert to the appropriate exception.
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(sqlException);
            if (exceptionHandler.IsForeignKeyNotFound)
            {
                string propertyName = string.Empty;
                string errorMessage = ErrorMessages.NotFound;
                switch (exceptionHandler.ViolatedFieldName)
                {
                    case "user_id":
                        propertyName = nameof(consultant.CreatedUserId);
                        errorMessage = errorMessage
                            .ReplaceResourceName(DisplayNames.User);
                        break;
                    case "customer_id":
                        propertyName = nameof(consultant.CustomerId);
                        errorMessage = errorMessage
                            .ReplaceResourceName(DisplayNames.Customer);
                        break;
                }
                throw new OperationException(propertyName, errorMessage);
            }
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure it exists.
        Consultant consultant = await _context.Consultants
            .SingleOrDefaultAsync(c => c.Id == id && !c.IsDeleted)
            ?? throw new ResourceNotFoundException(
                nameof(Consultant),
                nameof(id),
                id.ToString());
        
        // Remove expense.
        _context.Consultants.Remove(consultant);
        
        // Perform the deleting operation.
        try
        {
            await _context.SaveChangesAsync();

            // The expense can be deleted sucessfully without any error, revert the stats.
            await _statsService.IncrementConsultantGrossRevenueAsync(
                - consultant.Amount,
                DateOnly.FromDateTime(consultant.PaidDateTime));
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlExecption)
        {
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(sqlExecption);
            if (exceptionHandler.IsDeleteOrUpdateRestricted)
            {
                string errorMessage = ErrorMessages.DeleteRestricted
                    .ReplaceResourceName(DisplayNames.Expense);
                throw new OperationException(errorMessage);
            }
            
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }
    
    /// <summary>
    /// Lg the update history for the specified consultant entity.
    /// </summary>
    /// <param name="consultant">The consultant to be logged a new update history.</param>
    /// <param name="oldData">The old data before updating of the consultant.</param>
    /// <param name="newData">The new data after updating of the consultant.</param>
    /// <param name="reason">The reason of the modification.</param>
    private void LogUpdateHistory(
            Consultant consultant,
            ConsultantUpdateHistoryDataDto oldData,
            ConsultantUpdateHistoryDataDto newData,
            string reason)
    {
        ConsultantUpdateHistory updateHistory = new ConsultantUpdateHistory
        {
            UpdatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId()
        };
        
        if (consultant.UpdateHistories == null)
        {
            consultant.UpdateHistories = new List<ConsultantUpdateHistory>();
        }
        consultant.UpdateHistories.Add(updateHistory);
    }
}