namespace NATSInternal.Services;

/// <inheritdoc />
public class CountryService : ICountryService
{
    private readonly DatabaseContext _context;

    public CountryService(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<CountryListResponseDto> GetListAsync()
    {
        return new CountryListResponseDto
        {
            Items = await _context.Countries
                .OrderBy(c => c.Id)
                .Select(c => new CountryResponseDto(c))
                .ToListAsync()
        };
    }
}
