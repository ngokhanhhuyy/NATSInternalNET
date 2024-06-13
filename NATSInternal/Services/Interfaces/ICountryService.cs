namespace NATSInternal.Services.Interfaces;

public interface ICountryService
{    
    /// <summary>
    /// Get list of all countries information.
    /// </summary>
    /// <returns>A list object containing all countries' information.</returns>
    Task<CountryListResponseDto> GetListAsync();
}
