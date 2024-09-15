namespace NATSInternal.Services;

/// <inheritdoc />
public class ProductService : IProductService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly ISupplyService _supplyService;
    private readonly IOrderService _orderService;
    private readonly ITreatmentService _treatmentService;
    private readonly IAuthorizationService _authorizationService;

    public ProductService(
            DatabaseContext context,
            IPhotoService photoService,
            ISupplyService supplyService,
            IOrderService orderService,
            ITreatmentService treatmentService,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _photoService = photoService;
        _supplyService = supplyService;
        _orderService = orderService;
        _treatmentService = treatmentService;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<ProductListResponseDto> GetListAsync(ProductListRequestDto requestDto)
    {
        IQueryable<Product> query = _context.Products
            .OrderBy(p => p.CreatedDateTime);

        // Filter by category name.
        if (requestDto.CategoryName != null)
        {
            query = query
                .Where(p => p.Category != null && p.Category.Name == requestDto.CategoryName);
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
            query = query
                .Where(p => p.Name.ToLower().Contains(productNonDiacriticsName.ToLower()));
        }

        ProductListResponseDto responseDto = new ProductListResponseDto
        {
            Authorization = _authorizationService.GetProductListAuthorization()
        };
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling(
            (double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(p => new ProductBasicResponseDto(
                p,
                _authorizationService.GetProductAuthorization(p)))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<ProductDetailResponseDto> GetDetailAsync(
            int id,
            ProductDetailRequestDto requestDto)
    {
        Product product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Product),
                nameof(id),
                id.ToString());

        SupplyListResponseDto supplyListResponseDto = await _supplyService
            .GetListAsync(new SupplyListRequestDto
            {
                OrderByAscending = false,
                OrderByField = nameof(SupplyListRequestDto.FieldOptions.PaidDateTime),
                Page = 1,
                ResultsPerPage = requestDto.RecentSuppliesResultCount
            });
        OrderListResponseDto orderListResponseDto = await _orderService
            .GetListAsync(new OrderListRequestDto
            {
                OrderByAscending = false,
                OrderByField = nameof(OrderListRequestDto.FieldOptions.PaidDateTime),
                Page = 1,
                ResultsPerPage = requestDto.RecentOrdersResultCount
            });
        TreatmentListResponseDto treatmentListResponseDto = await _treatmentService
            .GetListAsync(new TreatmentListRequestDto
            {
                OrderByAscending = false,
                OrderByField = nameof(TreatmentListRequestDto.FieldOptions.PaidDateTime),
                Page = 1,
                ResultsPerPage = requestDto.RecentOrdersResultCount
            });

        ProductAuthorizationResponseDto authorizationResponseDto = _authorizationService
            .GetProductAuthorization(product);

        return new ProductDetailResponseDto(
            product,
            supplyListResponseDto.Items,
            orderListResponseDto.Items,
            treatmentListResponseDto.Items,
            authorizationResponseDto);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(ProductUpsertRequestDto requestDto)
    {
        // Initialize product entity.
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
            BrandId = requestDto.BrandId,
            CategoryId = requestDto.CategoryId,
            Photos = new List<ProductPhoto>()
        };
        _context.Products.Add(product);

        // Create thumbnail if specified.
        if (requestDto.ThumbnailFile != null)
        {
            string thumbnailUrl = await _photoService
                .CreateAsync(requestDto.ThumbnailFile, "products", true);
            product.ThumbnailUrl = thumbnailUrl;
        }

        // Create photos if specified.
        if (requestDto.Photos != null)
        {
            await CreatePhotosAsync(product, requestDto.Photos);
        }

        try
        {
            await _context.SaveChangesAsync();
            return product.Id;
        }
        catch (DbUpdateException exception)
        {
            // Delete the recently created thumbnail if exists.
            if (product.ThumbnailUrl != null)
            {
                _photoService.Delete(product.ThumbnailUrl);
            }

            // Delete the recently created photos if exists.
            if (product.Photos != null && product.Photos.Any())
            {
                foreach (ProductPhoto photo in product.Photos)
                {
                    _photoService.Delete(photo.Url);
                }
            }

            
            // Handle the concurrency-related operation.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handle the business-logic-related exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleDeleteOrUpdateException(sqlException);
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
        product.CategoryId = requestDto.CategoryId;
        product.BrandId = requestDto.BrandId;
        product.UpdatedDateTime = DateTime.UtcNow.ToApplicationTime();

        // Prepare lists of urls to be deleted later when the operation
        // is succeeded or failed.
        List<string> urlsToBeDeletedWhenFailed = new List<string>();
        List<string> urlsToBeDeletedWhenSucceeded = new List<string>();

        // Update the thumbnail if changed.
        if (requestDto.ThumbnailChanged)
        {
            // Delete the current thumbnail if exists.
            if (product.ThumbnailUrl != null)
            {
                urlsToBeDeletedWhenSucceeded.Add(product.ThumbnailUrl);
            }

            // Create a new one if the request contains data for it.
            if (requestDto.ThumbnailFile != null)
            {
                string thumbnailUrl = await _photoService
                    .CreateAsync(requestDto.ThumbnailFile, "products", true);
                product.ThumbnailUrl = thumbnailUrl;
                urlsToBeDeletedWhenFailed.Add(product.ThumbnailUrl);
            }
        }

        // Update the photos if changed.
        if (requestDto.Photos != null && requestDto.Photos.Any())
        {
            (List<string>, List<string>) photoUpdateResult;
            photoUpdateResult = await UpdatePhotosAsync(product, requestDto.Photos);
            urlsToBeDeletedWhenSucceeded.AddRange(photoUpdateResult.Item1);
            urlsToBeDeletedWhenFailed.AddRange(photoUpdateResult.Item2);
        }

        // Save changes or throw exeption if any error occurs.
        try
        {
            await _context.SaveChangesAsync();

            // The product can be updated successfully.
            // Delete the specified thumbnail and associated photos.
            foreach (string url in urlsToBeDeletedWhenSucceeded)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception)
        {
            // Delete the recently added thumbnail and photos.
            foreach (string url in urlsToBeDeletedWhenFailed)
            {
                _photoService.Delete(url);
            }
            
            // Handle the concurrency-related operation.
            if (exception is DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }

            // Handle the business-logic-related exception.
            if (exception.InnerException is MySqlException sqlException)
            {
                HandleDeleteOrUpdateException(sqlException);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity from the database and ensure the entity exists.
        Product product = await _context.Products
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new ResourceNotFoundException();

        // Remove the product and all associated photos.
        _context.Products.Remove(product);

        foreach (ProductPhoto photo in product.Photos)
        {
            _context.ProductPhotos.Remove(photo);
        }

        // Performing deleting operation.
        try
        {
            await _context.SaveChangesAsync();

            // The product can be deleted successfully.
            // Delete the thumbnail.
            if (product.ThumbnailUrl != null)
            {
                _photoService.Delete(product.ThumbnailUrl);
            }

            // Delete the photos.
            foreach (ProductPhoto photo in product.Photos)
            {
                _photoService.Delete(photo.Url);
            }
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
                SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
                exceptionHandler.Handle(sqlException);
                // Entity is referenced by some other column's entity, perform soft delete
                // instead.
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
        // Handle foreign key exception.
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            string errorMessage;
            if (exceptionHandler.ViolatedFieldName == "brand_id")
            {
                errorMessage = ErrorMessages.NotFound
                    .ReplaceResourceName(DisplayNames.Get(nameof(Brand)));
                throw new OperationException(errorMessage);
            }

            errorMessage = ErrorMessages.NotFound
                .ReplaceResourceName(DisplayNames.Get(nameof(ProductCategory)));

            throw new OperationException(errorMessage);
        }

        // Handle unique conflict exception.
        if (exceptionHandler.IsUniqueConstraintViolated)
        {
            string errorMessage = ErrorMessages.UniqueDuplicated
                .ReplacePropertyName(DisplayNames.Get(nameof(Product.Name)));
            throw new OperationException(nameof(Product.Name), errorMessage);
        }
    }

    /// <summary>
    /// Create photos which are associated to the specified product with the data
    /// provided in the request.
    /// </summary>
    /// <param name="product">
    /// The <c>Product</c> entity to which the photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// A list of objects containing the data for the new photos.
    /// </param>
    /// <returns>
    /// A <c>Task</c> object reprensenting the asynchronous operation.
    /// </returns>
    private async Task CreatePhotosAsync(
            Product product,
            List<ProductPhotoRequestDto> requestDtos)
    {
        foreach (ProductPhotoRequestDto requestDto in requestDtos)
        {
            string url = await _photoService.CreateAsync(
                requestDto.File, "products", false);
            ProductPhoto photo = new ProductPhoto
            {
                Url = url
            };
            product.Photos.Add(photo);
        }
    }

    /// <summary>
    /// Update the specified product's photos with the data provided in the request.
    /// </summary>
    /// <param name="product">
    /// The product to which the updating photos are associated.
    /// </param>
    /// <param name="requestDtos">
    /// An object containing the data for the photos to be updated.
    /// </param>
    /// <returns>
    /// A <see cref="Tuple"/> containing 2 lists of strings. The first one contains the urls
    /// of the photos which must be deleted when the update operation succeeded. The
    /// other one contains the urls of the photos which must be deleted when the
    /// updating operation failed.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when the photo with the given id which is associated to the specified
    /// product in the request cannot be found.
    /// </exception>
    private async Task<(List<string>, List<string>)> UpdatePhotosAsync(
            Product product,
            List<ProductPhotoRequestDto> requestDtos)
    {
        product.Photos ??= new List<ProductPhoto>();
        List<string> urlsToBeDeletedWhenSucceeded = new List<string>();
        List<string> urlsToBeDeletedWhenFailed = new List<string>();
        for (int i = 0; i < requestDtos.Count; i++)
        {
            ProductPhotoRequestDto photoRequestDto = requestDtos[i];
            if (photoRequestDto.HasBeenChanged)
            {
                // Fetch the photo entity and ensure it exists.
                ProductPhoto productPhoto = product.Photos
                    .SingleOrDefault(p => p.Id == photoRequestDto.Id);
                if (productPhoto == null)
                {
                    string errorMessage = ErrorMessages.NotFoundByProperty
                        .ReplaceResourceName(DisplayNames.ProductPhoto)
                        .ReplacePropertyName(DisplayNames.Id)
                        .ReplaceAttemptedValue(photoRequestDto.Id.ToString());
                    throw new OperationException($"photos[{i}].id", errorMessage);
                }

                // Add to list to be deleted later if the transaction succeeds.
                urlsToBeDeletedWhenSucceeded.Add(productPhoto.Url);
                product.Photos.Remove(productPhoto);

                if (photoRequestDto.HasBeenChanged)
                {
                    string url = await _photoService
                        .CreateAsync(photoRequestDto.File, "supplies", false);
                    // Add to list to be deleted later if the transaction fails.
                    urlsToBeDeletedWhenFailed.Add(url);
                    productPhoto.Url = url;
                }
            }
        }

        return (urlsToBeDeletedWhenSucceeded, urlsToBeDeletedWhenFailed);
    }
}