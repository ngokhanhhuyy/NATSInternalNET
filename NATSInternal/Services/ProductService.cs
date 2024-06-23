namespace NATSInternal.Services;

/// <inheritdoc />
public class ProductService : IProductService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;

    public ProductService(DatabaseContext context, IPhotoService photoService)
    {
        _context = context;
        _photoService = photoService;
    }

    /// <inheritdoc />
    public async Task<ProductListResponseDto> GetListAsync(ProductListRequestDto requestDto)
    {
        IQueryable<Product> query = _context.Products
            .OrderBy(p => p.CreatedDateTime);

        // Filter by category name.
        if (requestDto.CategoryName != null)
        {
            query = query.Where(p => p.Category != null && p.Category.Name == requestDto.CategoryName);
        }

        // Filter by brand id.
        if (requestDto.BrandId != null)
        {
            query = query.Where(p => p.BrandId == requestDto.BrandId);
        }

        // Filter by product name.
        if (requestDto.ProductName != null)
        {
            string productNonDiacriticsName = requestDto.ProductName.ToNonDiacritics();
            query = query.Where(p => p.Name.ToLower().Contains(productNonDiacriticsName.ToLower()));
        }

        ProductListResponseDto responseDto = new ProductListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(p => new ProductBasicResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Unit = p.Unit,
                Price = p.Price,
                StockingQuantity = p.StockingQuantity,
                ThumbnailUrl = p.ThumbnailUrl
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<ProductDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                Price = p.Price,
                VatFactor = p.VatFactor,
                StockingQuantity = p.StockingQuantity,
                IsForRetail = p.IsForRetail,
                IsDiscontinued = p.IsDiscontinued,
                CreatedDateTime = p.CreatedDateTime,
                UpdatedDateTime = p.UpdatedDateTime,
                ThumbnailUrl = p.ThumbnailUrl,
                Category = p.Category == null ? null : new ProductCategoryResponseDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                },
                Brand = p.Brand == null ? null : new BrandBasicResponseDto
                {
                    Id = p.Brand.Id,
                    Name = p.Brand.Name
                }
            }).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Product),
                nameof(id),
                id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(ProductUpsertRequestDto requestDto)
    {
        Product product = new Product
        {
            Name = requestDto.Name,
            Description = requestDto.Description,
            Unit = requestDto.Unit,
            Price = requestDto.Price,
            VatFactor = requestDto.VatFactor,
            IsForRetail = requestDto.IsForRetail,
            IsDiscontinued = requestDto.IsDiscontinued,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            UpdatedDateTime = null,
            StockingQuantity = 0,
            BrandId = requestDto.Brand == null ? null
                : requestDto.Brand.Id,
            CategoryId = requestDto.Category == null ? null
                : requestDto.Category.Id
        };

        if (requestDto.ThumbnailFile != null)
        {
            string thumbnailUrl = await _photoService
                .CreateAsync(requestDto.ThumbnailFile, "products", true);
            product.ThumbnailUrl = thumbnailUrl;
        }

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                HandleDeleteOrUpdateException(exception.InnerException as MySqlException);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, ProductUpsertRequestDto requestDto)
    {
        // Fetch the entity with given id from the database and ensure that it exists.
        Product product = await _context.Products.FindAsync(id)
            ?? throw new ResourceNotFoundException(
                nameof(Product),
                nameof(id),
                id.ToString());

        // Update the entity's properties.
        product.Name = requestDto.Name;
        product.Description = requestDto.Description;
        product.Unit = requestDto.Unit;
        product.Price = requestDto.Price;
        product.VatFactor = requestDto.VatFactor;
        product.IsForRetail = requestDto.IsForRetail;
        product.IsDiscontinued = requestDto.IsDiscontinued;
        product.CategoryId = requestDto.Category == null ? null : requestDto.Category.Id;
        product.BrandId = requestDto.Brand == null ? null : requestDto.Brand.Id;
        product.UpdatedDateTime = DateTime.UtcNow.ToApplicationTime();

        // Update the thumbnail if changed.
        if (requestDto.ThumbnailChanged)
        {
            // Delete the current thumbnail if exists.
            if (product.ThumbnailUrl != null)
            {
                _photoService.Delete(product.ThumbnailUrl);
            }

            // Create a new one if the request contains data for it.
            if (requestDto.ThumbnailFile != null)
            {
                string thumbnailUrl = await _photoService
                    .CreateAsync(requestDto.ThumbnailFile, "products", true);
                product.ThumbnailUrl = thumbnailUrl;
            }
        }

        // Save changes or throw exeption if any error occurs.
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                HandleDeleteOrUpdateException(exception.InnerException as MySqlException);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure the entity exists.
        Product product = await _context.Products.FindAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Product), nameof(id), id.ToString());

        // Performing deleting operation.
        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(exception.InnerException as MySqlException);
                // Entity is referenced by some other column's entity, perform soft delete instead.
                if (exceptionHandler.IsDeleteOrUpdateRestricted)
                {
                    product.IsDeleted = true;
                    await _context.SaveChangesAsync();
                    return;
                }
            }

            throw;
        }
    }

    /// <summary>
    /// Handle delete or update operation exception from the database and
    /// convert it into appropriate OperationException.
    /// </summary>
    /// <param name="exception">
    /// The inner exception of the DbUpdateExeception, thrown by the database
    /// after committing changes.
    /// </param>
    /// <exception cref="OperationException"></exception>
    private void HandleDeleteOrUpdateException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception.InnerException as MySqlException);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            string errorMessage;
            if (exceptionHandler.ViolatedFieldName == "brand_id")
            {
                errorMessage = ErrorMessages.NotFound
                    .ReplaceResourceName(DisplayNames.Get(nameof(Brand)));
                throw new OperationException(errorMessage);
            }
            else
            {
                errorMessage = ErrorMessages.NotFound
                    .ReplaceResourceName(DisplayNames.Get(nameof(Brand)));
            }

            throw new OperationException(errorMessage);
        }
        else if (exceptionHandler.IsUniqueConstraintViolated)
        {
            string errorMessage = ErrorMessages.UniqueDuplicated
                .ReplacePropertyName(DisplayNames.Get(nameof(Product.Name)));
            throw new OperationException(nameof(Product.Name), errorMessage);
        }
    }
}
