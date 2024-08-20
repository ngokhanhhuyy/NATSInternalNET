namespace NATSInternal.Services;

/// <inheritdoc />
public class CustomerService : ICustomerService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;

    public CustomerService(
        DatabaseContext context,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<CustomerListResponseDto> GetListAsync(CustomerListRequestDto requestDto)
    {
        // Initialize query.
        IQueryable<Customer> query = _context.Customers
            .Include(c => c.Debts)
            .Include(c => c.DebtPayments)
            .Where(c => !c.IsDeleted);

        // Determine the field and the direction the sort.
        switch (requestDto.OrderByField)
        {
            case nameof(CustomerListRequestDto.FieldToBeOrdered.FirstName):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(c => c.FirstName)
                    : query.OrderByDescending(c => c.FirstName);
                break;
            case nameof(CustomerListRequestDto.FieldToBeOrdered.Birthday):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(c => c.Birthday)
                    : query.OrderByDescending(c => c.Birthday);
                break;
            case nameof(CustomerListRequestDto.FieldToBeOrdered.CreatedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(c => c.CreatedDateTime)
                    : query.OrderByDescending(c => c.CreatedDateTime);
                break;
            case nameof(CustomerListRequestDto.FieldToBeOrdered.DebtRemainingAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(c => c.Debts
                            .Where(d => !d.IsDeleted)
                            .Sum(d => d.Amount) - c.DebtPayments
                            .Where(dp => !dp.IsDeleted)
                            .Sum(dp => dp.Amount))
                        .ThenBy(c => c.Id)
                    : query.OrderByDescending(c => c.Debts
                            .Where(d => !d.IsDeleted)
                            .Sum(d => d.Amount) - c.DebtPayments
                            .Where(dp => !dp.IsDeleted)
                            .Sum(dp => dp.Amount))
                        .ThenByDescending(c => c.Id);
                break;
            default:
                query = requestDto.OrderByAscending
                    ? query.OrderBy(c => c.LastName)
                    : query.OrderByDescending(c => c.LastName);
                break;
        }

        // Filter by search content.
        if (requestDto.SearchByContent != null)
        {
            bool isValidBirthday = DateOnly.TryParse(requestDto.SearchByContent, out DateOnly birthday);
            query = query.Where(c =>
                c.NormalizedFullName.Contains(requestDto.SearchByContent.ToUpper()) ||
                c.PhoneNumber.Contains(requestDto.SearchByContent) ||
                (isValidBirthday && c.Birthday.HasValue && c.Birthday.Value == birthday));
        }
        
        // Filter by remaining debt amount.
        if (requestDto.HasRemainingDebtAmountOnly)
        {
            query = query.Where(c => c.Debts
                .Where(d => !d.IsDeleted)
                .Sum(d => d.Amount) - c.DebtPayments
                .Where(dp => !dp.IsDeleted)
                .Sum(dp => dp.Amount) > 0);
        }

        // Initialize response dto.
        CustomerListResponseDto responseDto = new CustomerListResponseDto
        {
            Authorization = _authorizationService.GetCustomerListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Results = await query
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .Select(c => new CustomerBasicResponseDto(
                c,
                _authorizationService.GetCustomerAuthorization(c)))
            .AsSplitQuery()
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<CustomerBasicResponseDto> GetBasicAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Debts)
            .Include(c => c.DebtPayments)
            .Where(c => c.Id == id)
            .Select(c => new CustomerBasicResponseDto(
                c,
                _authorizationService.GetCustomerAuthorization(c)))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Customer),
                nameof(id),
                id.ToString());
    }

    /// <inheritdoc />
    public async Task<CustomerDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Introducer)
            .Include(c => c.Debts)
            .Include(c => c.DebtPayments)
            .Where(c => !c.IsDeleted && c.Id == id)
            .Select(c => new CustomerDetailResponseDto(
                c,
                _authorizationService))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Customer),
                nameof(id),
                id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(CustomerUpsertRequestDto requestDto)
    {
        string fullName = PersonNameUtility.GetFullNameFromNameElements(
            requestDto.FirstName,
            requestDto.MiddleName,
            requestDto.LastName);

        Customer customer = new Customer
        {
            FirstName = requestDto.FirstName,
            NormalizedFirstName = requestDto.FirstName.ToNonDiacritics().ToUpper(),
            MiddleName = requestDto.MiddleName,
            NormalizedMiddleName = requestDto.MiddleName?.ToNonDiacritics().ToUpper(),
            LastName = requestDto.LastName,
            NormalizedLastName = requestDto.LastName?.ToNonDiacritics().ToUpper(),
            FullName = fullName,
            NormalizedFullName = fullName.ToNonDiacritics().ToUpper(),
            NickName = requestDto.NickName,
            Gender = requestDto.Gender,
            Birthday = requestDto.Birthday,
            PhoneNumber = requestDto.PhoneNumber,
            ZaloNumber = requestDto.ZaloNumber,
            FacebookUrl = requestDto.FacebookUrl,
            Email = requestDto.Email,
            Address = requestDto.Address,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            Note = requestDto.Note,
            IntroducerId = null,
            CreatedUserId = _authorizationService.GetUserId()
        };
        _context.Customers.Add(customer);
        try
        {
            await _context.SaveChangesAsync();
            return customer.Id;
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException sqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                if (exceptionHandler.IsForeignKeyNotFound)
                {
                    throw new OperationException(
                        nameof(requestDto.IntroducerId),
                        ErrorMessages.NotFound
                            .Replace("{ResourceName}", DisplayNames.Introducer));
                }
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, CustomerUpsertRequestDto requestDto)
    {
        string fullName = PersonNameUtility.GetFullNameFromNameElements(
            requestDto.FirstName,
            requestDto.MiddleName,
            requestDto.LastName);

        try
        {
            int affactedRows = await _context.Customers
                .Where(c => !c.IsDeleted && c.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.FirstName, requestDto.FirstName)
                    .SetProperty(c => c.NormalizedFirstName, requestDto.FirstName
                        .ToNonDiacritics()
                        .ToUpper())
                    .SetProperty(c => c.MiddleName, requestDto.MiddleName)
                    .SetProperty(c => c.NormalizedMiddleName, requestDto.MiddleName
                        .ToNonDiacritics()
                        .ToUpper())
                    .SetProperty(c => c.LastName, requestDto.LastName)
                    .SetProperty(c => c.NormalizedLastName, requestDto.LastName
                        .ToNonDiacritics()
                        .ToUpper())
                    .SetProperty(c => c.FullName, fullName)
                    .SetProperty(c => c.NormalizedFullName, fullName
                        .ToNonDiacritics()
                        .ToUpper())
                    .SetProperty(c => c.NickName, requestDto.NickName)
                    .SetProperty(c => c.Gender, requestDto.Gender)
                    .SetProperty(c => c.Birthday, requestDto.Birthday)
                    .SetProperty(c => c.PhoneNumber, requestDto.PhoneNumber)
                    .SetProperty(c => c.ZaloNumber, requestDto.ZaloNumber)
                    .SetProperty(c => c.FacebookUrl, requestDto.FacebookUrl)
                    .SetProperty(c => c.Email, requestDto.Email)
                    .SetProperty(c => c.Address, requestDto.Address)
                    .SetProperty(c => c.UpdatedDateTime, DateTime.UtcNow.ToApplicationTime())
                    .SetProperty(c => c.Note, requestDto.Note)
                    .SetProperty(c => c.IntroducerId, requestDto.IntroducerId.HasValue
                        ? requestDto.IntroducerId.Value
                        : null));
            
            // Check if the entity has been updated.
            if (affactedRows == 0)
            {
                throw new ResourceNotFoundException(
                    nameof(Customer),
                    nameof(id),
                    id.ToString());
            }
        }
        catch (MySqlException exception)
        {
            SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
            exceptionHandler.Handle(exception);
            if (exceptionHandler.IsForeignKeyNotFound)
            {
                throw new OperationException(
                    nameof(requestDto.IntroducerId),
                    ErrorMessages.NotFound
                        .Replace("{ResourceName}", DisplayNames.Introducer));
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        int affectedRows = await _context.Customers
            .Where(c => !c.IsDeleted && c.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.UpdatedDateTime, DateTime.UtcNow.ToApplicationTime())
                .SetProperty(c => c.IntroducerId, (int?)null)
                .SetProperty(c => c.IsDeleted, true));

        if (affectedRows == 0)
        {
            throw new ResourceNotFoundException(
                nameof(Customer),
                nameof(id),
                id.ToString());
        }
    }
}
