namespace NATSInternal.Services;

/// <inheritdoc cref="IExpenseService" />
public class ExpenseService : LockableEntityService, IExpenseService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
    private static MonthYearResponseDto _earliestRecordedMonthYear = null;
    
    public ExpenseService(
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

    /// <inheritdoc/>
    public async Task<ExpenseListResponseDto> GetListAsync(ExpenseListRequestDto requestDto)
    {
        // Initialize list of month and year options.
        if (_earliestRecordedMonthYear == null)
        {
            _earliestRecordedMonthYear = await _context.Expenses
                .OrderBy(s => s.PaidDateTime)
                .Select(s => new MonthYearResponseDto
                {
                    Year = s.PaidDateTime.Year,
                    Month = s.PaidDateTime.Month
                }).FirstOrDefaultAsync();
        }
        List<MonthYearResponseDto> monthYearOptions;
        monthYearOptions = GenerateMonthYearOptions(_earliestRecordedMonthYear);

        // Initialize query.
        IQueryable<Expense> query = _context.Expenses
            .Include(e => e.Photos);
        
        // Sorting direction and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(ExpenseListRequestDto.FieldOptions.Amount):
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

        // Filter by month and year if specified.
        if (requestDto.Month.HasValue && requestDto.Year.HasValue)
        {
            DateTime startDateTime = new DateTime(requestDto.Year.Value, requestDto.Month.Value, 1);
            DateTime endDateTime = startDateTime.AddMonths(1);
            query = query.Where(s => s.PaidDateTime >= startDateTime && s.PaidDateTime < endDateTime);
        }

        // Filter by category.
        if (requestDto.Category.HasValue)
        {
            query = query.Where(e => e.Category == requestDto.Category.Value);
        }
        
        // Initialize response dto.
        ExpenseListResponseDto responseDto = new ExpenseListResponseDto
        {
            MonthYearOptions = monthYearOptions,
            Authorization = _authorizationService.GetExpenseListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(e => new ExpenseBasicResponseDto(
                e,
                _authorizationService.GetExpenseAuthorization(e)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        
        return responseDto;
    }

    /// <inheritdoc/>
    public async Task<ExpenseDetailResponseDto> GetDetailAsync(int id)
    {
        // Initialize query.
        IQueryable<Expense> query = _context.Expenses
            .Include(e => e.CreatedUser).ThenInclude(u => u.Roles)
            .Include(e => e.Payee)
            .Include(e => e.Photos);

        // Determine if the update histories should be fetched.
        bool shouldIncludeUpdateHistories = _authorizationService
            .CanAccessExpenseUpdateHistories();
        if (shouldIncludeUpdateHistories)
        {
            query = query.Include(e => e.UpdateHistories);
        }

        // Fetch the entity with the given id and ensure it exists in the database.
        Expense expense = await query
            .AsSingleQuery()
            .SingleOrDefaultAsync(e => e.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(Expense),
                nameof(id),
                id.ToString());
        
        return new ExpenseDetailResponseDto(
            expense,
            _authorizationService.GetExpenseAuthorization(expense),
            mapHistories: shouldIncludeUpdateHistories);
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(ExpenseUpsertRequestDto requestDto)
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
            
            paidDateTime = requestDto.PaidDateTime.Value;
        }
        
        // Initialize entity.
        Expense expense = new Expense
        {
            Amount = requestDto.Amount,
            PaidDateTime = paidDateTime,
            Category = requestDto.Category,
            Note = requestDto.Note,
            CreatedUserId = _authorizationService.GetUserId(),
            Photos = new List<ExpensePhoto>()
        };
        _context.Expenses.Add(expense);
        
        // Set expense payee
        ExpensePayee payee = await _context.ExpensePayees
            .Where(ep => ep.Name == requestDto.PayeeName)
            .SingleOrDefaultAsync();
        
        if (payee == null)
        {
            payee = new ExpensePayee
            {
                Name = requestDto.PayeeName
            };
            expense.Payee = payee;
        }
        
        // Create expense photos.
        if (requestDto.Photos != null)
        {
            await CreatePhotosAsync(expense, requestDto.Photos);
        }
        
        // Perform the creating operation.
        try
        {
            await _context.SaveChangesAsync();

            // Expense can be created successfully, adjust the stats.
            DateOnly paidDate = DateOnly.FromDateTime(paidDateTime);
            await _statsService.IncrementExpenseAsync(expense.Amount, expense.Category, paidDate);

            // Commit the transaction, finishing the operation.
            await transaction.CommitAsync();
            return expense.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            // Remove all created photos.
            foreach (string url in expense.Photos.Select(p => p.Url))
            {
                _photoService.Delete(url);
            }
            
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
                        propertyName = nameof(expense.CreatedUserId);
                        errorMessage = errorMessage.ReplaceResourceName(DisplayNames.User);
                        break;
                    case "payee_id":
                        propertyName = nameof(expense.PayeeId);
                        errorMessage = errorMessage.ReplaceResourceName(DisplayNames.ExpensePayee);
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
    public async Task UpdateAsync(int id, ExpenseUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Expense expense = await _context.Expenses
            .Include(e => e.CreatedUser)
            .Include(e => e.Payee)
            .Include(e => e.Photos)
            .Where(e => e.Id == id)
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(nameof(Expense), nameof(id), id.ToString());
        
        // Ensure the entity is editable by the requester.
        if (!_authorizationService.CanEditExpense(expense))
        {
            throw new AuthorizationException();
        }

        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Storing the old data for update history logging and stats adjustment.
        long oldAmount = expense.Amount;
        ExpenseCategory oldCategory = expense.Category;
        DateOnly oldPaidDate = DateOnly.FromDateTime(expense.PaidDateTime);
        ExpenseUpdateHistoryDataDto oldData = new ExpenseUpdateHistoryDataDto(expense);

        // Determine the PaidDateTime if the request has specified a value.
        if (requestDto.PaidDateTime.HasValue)
        {
            // Check if the current user has permission to specify the paid datetime.
            if (!_authorizationService.CanSetExpensePaidDateTime())
            {
                throw new AuthorizationException();
            }

            // Prevent the consultant's PaidDateTime to be modified when the consultant is locked.
            if (expense.IsLocked)
            {
                string errorMessage = ErrorMessages.CannotSetDateTimeAfterLocked
                    .ReplaceResourceName(DisplayNames.Consultant)
                    .ReplacePropertyName(DisplayNames.PaidDateTime);
                throw new OperationException(
                    nameof(requestDto.PaidDateTime),
                    errorMessage);
            }

            // Assign the new PaidDateTime value only if it's different from the old one.
            if (requestDto.PaidDateTime.Value != expense.PaidDateTime)
            {
                // Validate and assign the specified PaidDateTime value from the request.
                try
                {
                    _statsService.ValidateStatsDateTime(expense, requestDto.PaidDateTime.Value);
                    expense.PaidDateTime = requestDto.PaidDateTime.Value;
                }
                catch (ArgumentException exception)
                {
                    string errorMessage = exception.Message
                        .ReplacePropertyName(DisplayNames.PaidDateTime);
                    throw new OperationException(
                        nameof(requestDto.PaidDateTime),
                        errorMessage);
                }
            }
        }
        
        // Update other fields.
        expense.Amount = requestDto.Amount;
        expense.Category = requestDto.Category;
        expense.Note = requestDto.Note;
        
        // Update payee.
        if (expense.Payee.Name != requestDto.PayeeName)
        {
            ExpensePayee payee = await _context.ExpensePayees
                .SingleOrDefaultAsync(ep => ep.Name == requestDto.PayeeName)
                ?? new ExpensePayee { Name = requestDto.PayeeName };
            
            // Remove old payee if there is no other expeses consuming it.
            bool isCurrentPayeeConsumed = await _context.Expenses
                .AnyAsync(e => e.Id != expense.Id && e.Payee.Id == expense.Payee.Id);
            if (!isCurrentPayeeConsumed)
            {
                _context.ExpensePayees.Remove(expense.Payee);
            }
            
            // Set new payee.
            expense.Payee = payee;
        }
        
        // Update photos.
        List<string> urlsToBeDeletedWhenSucceeded = new List<string>();
        List<string> urlsToBeDeletedWhenFailed = new List<string>();
        if (requestDto.Photos != null)
        {
            (List<string>, List<string>) photosUpdateResult;
            photosUpdateResult = await UpdatePhotosAsync(expense, requestDto.Photos);
            urlsToBeDeletedWhenSucceeded.AddRange(photosUpdateResult.Item1);
            urlsToBeDeletedWhenFailed.AddRange(photosUpdateResult.Item2);
        }
        
        // Storing the new data for update history logging.
        ExpenseUpdateHistoryDataDto newData = new ExpenseUpdateHistoryDataDto(expense);
        
        // Log update history.
        LogUpdateHistory(expense, oldData, newData, requestDto.UpdateReason);
        
        // Perform the updating operation.
        try
        {
            await _context.SaveChangesAsync();
            
            // The expense can be saved without error.
            // Revert the old stats.
            await _statsService.IncrementExpenseAsync(
                -oldAmount, oldCategory, oldPaidDate);

            // Add the new stats.
            DateOnly newPaidDate = DateOnly.FromDateTime(expense.PaidDateTime);
            await _statsService.IncrementExpenseAsync(
                expense.Amount, expense.Category, newPaidDate);
            
            // Commit the transaction, finish the opeartion.
            await transaction.CommitAsync();
            
            // Clean the old photos.
            foreach (string url in urlsToBeDeletedWhenSucceeded)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        {
            // Remove all created photos.
            foreach (string url in urlsToBeDeletedWhenFailed)
            {
                _photoService.Delete(url);
            }
            
            // Handle concurrency exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handle operation exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                if (exceptionHandler.IsForeignKeyNotFound)
                {
                    string propertyName = string.Empty;
                    string errorMessage = ErrorMessages.NotFound;
                    switch (exceptionHandler.ViolatedFieldName)
                    {
                        case "created_user_id":
                            propertyName = nameof(expense.CreatedUserId);
                            errorMessage = errorMessage
                                .ReplaceResourceName(DisplayNames.User);
                            break;
                        case "payee_id":
                            propertyName = nameof(expense.PayeeId);
                            errorMessage = errorMessage
                                .ReplaceResourceName(DisplayNames.ExpensePayee);
                            break;
                    }
                    throw new OperationException(propertyName, errorMessage);
                }
            }
            
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure it exists.
        Expense expense = await _context.Expenses
            .Include(e => e.Payee)
            .Include(e => e.Photos)
            .SingleOrDefaultAsync(e => e.Id == id)
            ?? throw new ResourceNotFoundException(nameof(Expense), nameof(id), id.ToString());
        
        // Ensure the user has permission to delete this expense.
        if (!_authorizationService.CanDeleteExpense(expense))
        {
            throw new AuthorizationException();
        }
        
        // Remove expense.
        _context.Expenses.Remove(expense);
        
        // Remove expense payee.
        bool isCurrentPayeeConsumed = await _context.Expenses
            .Where(e => e.Id != expense.Id && e.Payee.Name == expense.Payee.Name)
            .AnyAsync();
        if (!isCurrentPayeeConsumed)
        {
            _context.ExpensePayees.Remove(expense.Payee);
        }
        
        // Remove expense photos.
        List<string> photoUrlsToBeDeletedWhenSucceeded = new List<string>();
        foreach (ExpensePhoto photo in expense.Photos)
        {
            photoUrlsToBeDeletedWhenSucceeded.Add(photo.Url);
            _context.ExpensePhotos.Remove(photo);
        }
        
        // Save changes.
        try
        {
            await _context.SaveChangesAsync();

            // The expense can be deleted sucessfully without any error, adjust the stats.
            await _statsService.IncrementExpenseAsync(
                - expense.Amount,
                expense.Category,
                DateOnly.FromDateTime(expense.PaidDateTime));

            // Remove all expense photos.
            foreach (string url in photoUrlsToBeDeletedWhenSucceeded)
            {
                _photoService.Delete(url);
            }
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
    /// Create photos with the data provided in the request for the specified <c>Expense</c>.
    /// </summary>
    /// <param name="expense">
    /// The <c>Expense</c> of which the photos are to be created.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data of the new photos.
    /// </param>
    private async Task CreatePhotosAsync(
            Expense expense,
            List<ExpensePhotoRequestDto> requestDtos)
    {
        expense.Photos ??= new List<ExpensePhoto>();
        foreach (ExpensePhotoRequestDto photoRequestDto in requestDtos)
        {
            string url = await _photoService
                .CreateAsync( photoRequestDto.File, "expenses", false);
            ExpensePhoto photo = new ExpensePhoto
            {
                Url = url
            };
            expense.Photos.Add(photo);
        }
    }
    
    /// <summary>
    /// Update the specified <c>Expense</c>'s photos with the data provided in the request.
    /// </summary>
    /// <param name="expense">
    /// The <c>Expense</c> of which the photos are to be updated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data for the photos to be updated.
    /// </param>
    /// <returns>
    /// A <c>Tuple</c> which contains 2 lists of urls. The first list contains the urls
    /// which must be deleted after the expense updating operation succeeded. The other
    /// contains the urls which must be deleted after the expense updating operation failed
    /// and is aborted.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thown when the photo associated to one of the ids specified in the request doesn't exist.
    /// </exception>
    private async Task<(List<string>, List<string>)> UpdatePhotosAsync(
            Expense expense,
            List<ExpensePhotoRequestDto> requestDtos)
    {
        List<string> urlsToBeDeletedWhenSucceeded = new List<string>();
        List<string> urlsToBeDeletedWhenFailed = new List<string>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            ExpensePhotoRequestDto photoRequestDto = requestDtos[i];
            ExpensePhoto photo;
            if (!photoRequestDto.Id.HasValue)
            {
                string url = await _photoService
                    .CreateAsync(photoRequestDto.File, "expenses", false);
                urlsToBeDeletedWhenFailed.Add(url);
                photo = new ExpensePhoto { Url = url };
                expense.Photos.Add(photo);
            }
            else if (photoRequestDto.HasBeenChanged)
            {
                photo = expense.Photos.SingleOrDefault(e => e.Id == photoRequestDto.Id);
                if (photo == null)
                {
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplaceResourceName(DisplayNames.ExpensePhoto);
                    throw new OperationException($"photos[{i}].id", errorMessage);
                }

                // Delete old photo.
                urlsToBeDeletedWhenSucceeded.Add(photo.Url);

                // Create new photo.
                if (photoRequestDto.File != null)
                {
                    string url = await _photoService
                        .CreateAsync(photoRequestDto.File, "expenses", false);
                    urlsToBeDeletedWhenFailed.Add(url);
                    photo.Url = url;
                }
                else
                {
                    _context.ExpensePhotos.Remove(photo);
                }
            }
        }
        
        return (urlsToBeDeletedWhenSucceeded, urlsToBeDeletedWhenFailed);
    }
    
    /// <summary>
    /// Log the old and new data to update history for the specified expense.
    /// </summary>
    /// <param name="expense">
    /// The expense entity which the new update history is associated.
    /// </param>
    /// <param name="oldData">
    /// An object containing the old data of the expense before modification.
    /// </param>
    /// <param name="newData">
    /// An object containing the new data of the expense after modification. 
    /// </param>
    /// <param name="reason">The reason of the modification.</param>
    private void LogUpdateHistory(
            Expense expense,
            ExpenseUpdateHistoryDataDto oldData,
            ExpenseUpdateHistoryDataDto newData,
            string reason)
    {
        ExpenseUpdateHistory updateHistory = new ExpenseUpdateHistory
        {
            Reason = reason,
            OldData = JsonSerializer.Serialize(oldData),
            NewData = JsonSerializer.Serialize(newData),
            UserId = _authorizationService.GetUserId()
        };
        
        expense.UpdateHistories ??= new List<ExpenseUpdateHistory>();
        expense.UpdateHistories.Add(updateHistory);
    }
}