namespace NATSInternal.Services;

/// <interitdoc />
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

    public async Task<int> CreateAsyns(ProductCategoryRequestDto requestDto)
    {
        ProductCategory productCategory = new ProductCategory
        {
            Name = requestDto.Name,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime()
        };

        try
        {
            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();
            return productCategory.Id;
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                HandleException(exception.InnerException as MySqlException);
            }

            throw;
        }
    }

    public async Task UpdateAsync(int id, ProductCategoryRequestDto requestDto)
    {
        int affectedRows;
        try
        {
            affectedRows = await _context.ProductCategories
                .Where(pc => pc.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Name, requestDto.Name));

            if (affectedRows < 1)
            {
                throw new ResourceNotFoundException(
                    nameof(ProductCategory),
                    nameof(id),
                    id.ToString());
            }
        }
        catch (MySqlException exception)
        {
            HandleException(exception);
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        int affectedRows = await _context.ProductCategories
            .Where(pc => pc.Id == id)
            .ExecuteDeleteAsync();

        if (affectedRows < 1)
        {
            throw new ResourceNotFoundException(
                nameof(ProductCategory),
                nameof(id),
                id.ToString());
        }

        await transaction.CommitAsync();
    }

    private static void HandleException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsUniqueConstraintViolated)
        {
            string errorMessage = ErrorMessages.UniqueDuplicated
                .ReplacePropertyName(DisplayNames.Get(nameof(ProductCategory.Name)));
            throw new OperationException(errorMessage);
        }
    }
}
