namespace NATSInternal.Services;

/// <inheritdoc />
public class BrandService : IBrandService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;

    public BrandService(
            DatabaseContext context,
            IPhotoService photoService,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _photoService = photoService;
        _authorizationService = authorizationService;
    }

    public async Task<BrandListResponseDto> GetListAsync()
    {
        return new BrandListResponseDto
        {
            Items = await _context.Brands
                .OrderBy(b => b.Id)
                .Select(b => new BrandBasicResponseDto
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToListAsync(),
            Authorization = _authorizationService.GetBrandAuthorization()
        };
    }

    public async Task<BrandDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Brands
            .Include(b => b.Country)
            .Where(b => b.Id == id)
            .Select(b => new BrandDetailResponseDto
            {
                Id = b.Id,
                Name = b.Name,
                Website = b.Website,
                SocialMediaUrl = b.SocialMediaUrl,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                Address = b.Address,
                ThumbnailUrl = b.ThumbnailUrl,
                Country = b.Country == null ? null : new CountryResponseDto
                {
                    Id = b.Country.Id,
                    Name = b.Country.Name,
                    Code = b.Country.Code
                },
                Authorization = _authorizationService.GetBrandAuthorization()
            }).SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Brand),
                nameof(id),
                id.ToString());
    }

    public async Task<int> CreateAsync(BrandRequestDto requestDto)
    {
        string thumbnailUrl = null;
        if (requestDto.ThumbnailFile != null)
        {
            thumbnailUrl = await _photoService.CreateAsync(
                requestDto.ThumbnailFile,
                "brands",
                true);
        }

        Brand brand = new Brand
        {
            Name = requestDto.Name,
            Website = requestDto.Website,
            SocialMediaUrl = requestDto.SocialMediaUrl,
            PhoneNumber = requestDto.PhoneNumber,
            Email = requestDto.Email,
            Address = requestDto.Address,
            ThumbnailUrl = thumbnailUrl,
            CountryId = requestDto.Country == null ? null : requestDto.Country.Id
        };
        
        try
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand.Id;
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                HandleDbUpdateException(exception.InnerException as MySqlException);
            }
            throw;
        }
    }

    public async Task UpdateAsync(int id, BrandRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Brand brand = await _context.Brands.SingleOrDefaultAsync(b => b.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(Brand),
                nameof(id),
                id.ToString());

        // Update thumbnail if changed.
        string thumbnailUrl = brand.ThumbnailUrl;
        if (requestDto.ThumbnailChanged)
        {
            // Delete the current thumbnail if exists.
            if (brand.ThumbnailUrl != null)
            {
                _photoService.Delete(brand.ThumbnailUrl);
            }

            if (requestDto.ThumbnailFile != null)
            {
                thumbnailUrl = await _photoService.CreateAsync(
                    requestDto.ThumbnailFile,
                    "brands",
                    true);
            }
        }

        // Update the other properties.
        brand.Name = requestDto.Name;
        brand.Website = requestDto.Website;
        brand.SocialMediaUrl = requestDto.SocialMediaUrl;
        brand.PhoneNumber = requestDto.PhoneNumber;
        brand.Email = requestDto.Email;
        brand.Address = requestDto.Address;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException)
            {
                HandleDbUpdateException(exception.InnerException as MySqlException);
            }

            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        Brand brand = await _context.Brands.FindAsync(id)
            ?? throw new ResourceNotFoundException(
                nameof(Brand),
                nameof(id),
                id.ToString());

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();

        if (brand.ThumbnailUrl != null)
        {
            _photoService.Delete(brand.ThumbnailUrl);
        }

    }

    private void HandleDbUpdateException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        string errorMessage;
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            errorMessage = ErrorMessages.NotFound
                .ReplaceResourceName(DisplayNames.Country);
            throw new OperationException("country.id", errorMessage);
        }
        else if (exceptionHandler.IsUniqueConstraintViolated)
        {
            errorMessage = ErrorMessages.UniqueDuplicated
                .ReplaceResourceName(DisplayNames.Name);
            throw new OperationException("name", errorMessage);
        }
    }
}
