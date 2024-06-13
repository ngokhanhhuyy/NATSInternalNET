namespace NATSInternal.Services;

/// <inheritdoc />
public class CountryService : ICountryService
{
    private readonly DatabaseContext _context;

    public CountryService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<CountryListResponseDto> GetListAsync()
    {
        return new CountryListResponseDto
        {
            Items = await _context.Countries
                .OrderBy(c => c.Id)
                .Select(c => new CountryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code
                }).ToListAsync()
        };
    }
}
