namespace NATSInternal.Services.Interfaces;

public interface ISupplyService
{
    /// <summary>
    /// Get a list of supplies with filter options.
    /// </summary>
    /// <param name="requestDto">An object containing data for the options.</param>
    /// <returns>A list containing all supplies' data that meet the filter options.</returns>
    Task<SupplyListResponseDto> GetListAsync(SupplyListRequestDto requestDto);

    /// <summary>
    /// Get the detail information of the supply with given id.
    /// </summary>
    /// <param name="id">The id of the supply.</param>
    /// <returns>An object containing the detail information of the supply.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task<SupplyDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a supply with the data provided in the request.
    /// </summary>
    /// <param name="requestDto">An object containing the data for the </param>
    /// <returns></returns>
    /// <exception cref="OperationException"></exception>
    Task<int> CreateAsync(SupplyUpsertRequestDto requestDto);

    /// <summary>
    /// Update the supply which has the given id with the data provided from the request.
    /// </summary>
    /// <param name="id">The id of the supply to be updated.</param>
    /// <param name="requestDto">An object containing the data the be updated.</param>
    /// <exception cref="ResourceNotFoundException"></exception>
    /// <exception cref="OperationException"></exception>
    Task UpdateAsync(int id, SupplyUpsertRequestDto requestDto);

    /// <summary>
    /// Delete the supply with given id.
    /// </summary>
    /// <param name="id">The id of the supply to be deleted.</param>
    /// <exception cref="ResourceNotFoundException"></exception
    /// <exception cref="OperationException"></exception>
    Task DeleteAsync(int id);
}
