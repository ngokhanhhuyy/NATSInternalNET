namespace NATSInternal.Services;

/// <inheritdoc/>
public class ExpenseService : IExpenseService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;
    
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
        // Initialze query.
        IQueryable<Expense> query = _context.Expenses
            .Include(e => e.Photos);
        
        // Sorting direction and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(ExpenseListRequestDto.FieldOptions.Amount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(e => e.Amount).ThenBy(e => e.PaidDateTime)
                    : query.OrderByDescending(e => e.Amount).ThenBy(e => e.PaidDateTime);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(e => e.PaidDateTime).ThenBy(e => e.Amount)
                    : query.OrderByDescending(e => e.PaidDateTime).ThenBy(e => e.Amount);
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
        
        // Filter by category.
        if (requestDto.Category.HasValue)
        {
            query = query.Where(e => e.Category == requestDto.Category.Value);
        }
        
        
        // Initialize response dto.
        ExpenseListResponseDto responseDto = new ExpenseListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(e => new ExpenseBasicResponseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                PaidDateTime = e.PaidDateTime,
                Category = e.Category,
                IsClosed = e.IsClosed,
                Authorization = _authorizationService.GetExpenseAuthorization(e)
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();
        
        return responseDto;
    }

    /// <inheritdoc/>
    public async Task<ExpenseDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Expenses
            .Include(e => e.User).ThenInclude(u => u.Roles)
            .Include(e => e.Payee)
            .Include(e => e.Photos)
            .Where(e => e.Id == id)
            .Select(e => new ExpenseDetailResponseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                PaidDateTime = e.PaidDateTime,
                Category = e.Category,
                Note = e.Note,
                IsClosed = e.IsClosed,
                User = new UserBasicResponseDto
                {
                    Id = e.User.Id,
                    UserName = e.User.UserName,
                    FirstName = e.User.FirstName,
                    MiddleName = e.User.MiddleName,
                    LastName = e.User.LastName,
                    FullName = e.User.FullName,
                    Gender = e.User.Gender,
                    Birthday = e.User.Birthday,
                    JoiningDate = e.User.JoiningDate,
                    AvatarUrl = e.User.AvatarUrl,
                    Role = new RoleBasicResponseDto
                    {
                        Id = e.User.Role.Id,
                        Name = e.User.Role.Name,
                        DisplayName = e.User.Role.DisplayName,
                        PowerLevel = e.User.Role.PowerLevel
                    }
                },
                Payee = new ExpensePayeeResponseDto
                {
                    Id = e.Payee.Id,
                    Name = e.Payee.Name
                },
                Photos = e.Photos
                    .OrderBy(ep => ep.Id)
                    .Select(ep => new ExpensePhotoResponseDto
                    {
                        Id = ep.Id,
                        Url = ep.Url
                    }).ToList(),
                Authorization = _authorizationService.GetExpenseAuthorization(e)
            }).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(nameof(Expense), nameof(id), id.ToString());
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(ExpenseUpsertRequestDto requestDto)
    {
        // Use transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Initialize entity.
        Expense expense = new Expense
        {
            Amount = requestDto.Amount,
            PaidDateTime = requestDto.PaidDateTime ?? DateTime.UtcNow.ToApplicationTime(),
            Category = requestDto.Category,
            Note = requestDto.Note,
            Photos = new List<ExpensePhoto>()
        };
        
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
        
        // Set expense photos.
        if (requestDto.Photos != null)
        {
            foreach (ExpensePhotoRequestDto photoRequestDto in requestDto.Photos)
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
        
        // Set user.
        expense.UserId = _authorizationService.GetUserId();
        
        // Adjust stats.
        await _statsService.IncrementExpenseAsync(expense.Amount, expense.Category);
        
        // Save changes and commit.
        try
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
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
                        propertyName = nameof(expense.UserId);
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
            .Include(e => e.User)
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
        
        // Decrement previous stats.
        await _statsService.IncrementExpenseAsync(
            - expense.Amount,
            expense.Category,
            DateOnly.FromDateTime(expense.PaidDateTime));
        
        // Update expense and related entites.
        expense.Amount = requestDto.Amount;
        expense.Category = requestDto.Category;
        expense.Note = requestDto.Note;
        
        // Adjust new stats.
        await _statsService.IncrementExpenseAsync(
            expense.Amount,
            expense.Category,
            DateOnly.FromDateTime(expense.PaidDateTime));
        
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
        List<string> photoUrlsToBeDeletedWhenSucceeded = new List<string>();
        List<string> photoUrlsToBeDeletedWhenFailed = new List<string>();
        if (requestDto.Photos != null)
        {
            for (int i = 0; i < requestDto.Photos.Count; i++)
            {
                ExpensePhotoRequestDto photoRequestDto = requestDto.Photos[i];
                ExpensePhoto photo;
                if (!photoRequestDto.Id.HasValue)
                {
                    string url = await _photoService
                        .CreateAsync(photoRequestDto.File, "expenses", false);
                    photoUrlsToBeDeletedWhenFailed.Add(url);
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
                    photoUrlsToBeDeletedWhenSucceeded.Add(photo.Url);

                    // Create new photo.
                    if (photoRequestDto.File != null)
                    {
                        string url = await _photoService
                            .CreateAsync(photoRequestDto.File, "expenses", false);
                        photoUrlsToBeDeletedWhenFailed.Add(url);
                        photo.Url = url;
                    } else {
                        _context.ExpensePhotos.Remove(photo);
                    }
                }
            }
        }
        
        // Save changes and commit transaction.
        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            foreach (string url in photoUrlsToBeDeletedWhenSucceeded)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            // Remove all created photos.
            foreach (string url in photoUrlsToBeDeletedWhenFailed)
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
                        propertyName = nameof(expense.UserId);
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
            // Remove all created photos.
            foreach (string url in photoUrlsToBeDeletedWhenFailed)
            {
                _photoService.Delete(url);
            }
            
            throw new ConcurrencyException();
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
}