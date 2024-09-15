namespace NATSInternal.Services;

/// <inheritdoc />
public class ProductCategoryService : IProductCategoryService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;

    public ProductCategoryService(
            DatabaseContext context,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<ProductCategoryListResponseDto> GetListAsync()
    {
        return new ProductCategoryListResponseDto
        {
            Items = await _context.ProductCategories
                .OrderBy(pc => pc.Id)
                .Select(pc => new ProductCategoryResponseDto(pc))
                .ToListAsync(),
            Authorization = _authorizationService.GetProductCategoryAuthorization()
        };
    }

    /// <inheritdoc />
    public async Task<ProductCategoryResponseDto> GetDetailAsync(int id)
    {
        return await _context.ProductCategories
            .Where(p => p.Id == id)
            .Select(p => new ProductCategoryResponseDto(p))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(ProductCategory),
                nameof(id),
                id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsyns(ProductCategoryRequestDto requestDto)
    {
        // Initialize the entity.
        ProductCategory productCategory = new ProductCategory
        {
            Name = requestDto.Name,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime()
        };

        // Perform the creating operation.
        try
        {
            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();
            return productCategory.Id;
        }
        // Handle the exception.
        catch (DbUpdateException exception)
        {
            // Handle the concurrency-related exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handle the business-logic-related exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleException(sqlException);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, ProductCategoryRequestDto requestDto)
    {
        // Fetch the entity with the specified id from the database.
        ProductCategory productCategory = await _context.ProductCategories
            .SingleOrDefaultAsync(pc => pc.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(ProductCategory),
                nameof(id),
                id.ToString());
        
        // Update the property value.
        productCategory.Name = requestDto.Name;
        
        // Perform the updating operation.
        try
        {
            await _context.SaveChangesAsync();
        }
        // Handle the exception.
        catch (DbUpdateException exception)
        {
            // Handle the concurrency-related exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handle the business-logic-related exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleException(sqlException);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity with the specified id from the database.
        ProductCategory productCategory = await _context.ProductCategories
            .SingleOrDefaultAsync(pc => pc.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(ProductCategory),
                nameof(id),
                id.ToString());
        
        // Perform the deleting operation.
        try
        {
            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            // Handle the concurrency-related exception.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
            
            // Handle the business-logic-related exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleException(sqlException);
            }

            throw;
        }
    }

    /// <summary>
    /// Handle the exception which is thrown by the database during the creating or updating
    /// operation.
    /// </summary>
    /// <remarks>
    /// This method only convert the specified <c>exception</c> into the mapped
    /// <see cref="OperationException"/> under the defined circumstance.
    /// </remarks>
    /// <param name="exception">
    /// An instance of the <see cref="MySqlException"/> class, contanining the details of the
    /// error.
    /// </param>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the <c>exception</c> indicates that the error occurs due to the unique
    /// constraint violation during the operation.<br/>
    /// - When the <c>exception</c> indicates that the error occurs due to the restriction
    /// caused by the existence of some related resource(s).
    /// </exception>
    private static void HandleException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        string errorMessage;
        
        if (exceptionHandler.IsUniqueConstraintViolated)
        {
            errorMessage = ErrorMessages.UniqueDuplicated
                .ReplacePropertyName(DisplayNames.Get(nameof(ProductCategory.Name)));
            throw new OperationException(errorMessage);
        }
        
        if (exceptionHandler.IsDeleteOrUpdateRestricted)
        {
            errorMessage = ErrorMessages.DeleteRestricted
                .ReplaceResourceName(DisplayNames.Category);
            throw new OperationException(errorMessage);
        }
    }
}
