namespace NATSInternal.Services.Interfaces;

public interface IBrandService
{
    /// <summary>
    /// Get a list of all brands with basic information.
    /// </summary>
    /// <returns>An list object containing the basic information of all brands.</returns>
    Task<BrandListResponseDto> GetListAsync();

    /// <summary>
    /// Get the brand with detail information which has the given id.
    /// </summary>
    /// <param name="id">The id of the brand.</param>
    /// <returns>An object containing the detail information of the brand.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task<BrandDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a new brand with the data provided in the request.
    /// </summary>
    /// <param name="requestDto">An object containing the data for a new brand.</param>
    /// <returns>The id of the created brand.</returns>
    /// <exception cref="OperationException"></exception>
    Task<int> CreateAsync(BrandRequestDto requestDto);

    /// <summary>
    /// Update the brand with given id.
    /// </summary>
    /// <param name="id">The id of the brand to be updated.</param>
    /// <param name="requestDto">An object containing the data to be updated.</param>
    /// <exception cref="ResourceNotFoundException"></exception>
    /// <exception cref="OperationException"></exception>
    Task UpdateAsync(int id, BrandRequestDto requestDto);

    /// <summary>
    /// Delete the brand with given id.
    /// </summary>
    /// <param name="id">The id of the brand to be deleted.</param>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task DeleteAsync(int id);
}
