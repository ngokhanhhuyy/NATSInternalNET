namespace NATSInternal.Services.Interfaces;

public interface IProductCategoryService
{
    /// <summary>
    /// Get a list of available product categories.
    /// </summary>
    /// <returns>An object containing all available product categories.</returns>
    Task<ProductCategoryListResponseDto> GetListAsync();

    /// <summary>
    /// Get the detail information of the product category with given id.
    /// </summary>
    /// <param name="id">The id of the product category.</param>
    /// <returns>An object containing the detail of the product category.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task<ProductCategoryResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a new product category with the given data from the request.
    /// </summary>
    /// <param name="requestDto">An object containing the data for a new product category.</param>
    /// <returns>The id of the created product category.</returns>
    /// <exception cref="OperationException"></exception>
    Task<int> CreateAsyns(ProductCategoryRequestDto requestDto);

    /// <summary>
    /// Update the product category which has the given id with the given data from the request.
    /// </summary>
    /// <param name="id">The id of the product category to be updated.</param
    /// <param name="requestDto">
    /// An object containing the data for the product category to be updated.
    /// </param>
    /// <exception cref="ResourceNotFoundException"></exception>
    /// <exception cref="OperationException"></exception>
    Task UpdateAsync(int id, ProductCategoryRequestDto requestDto);

    /// <summary>
    /// Delete the product category with given id.
    /// </summary>
    /// <param name="id">The id of the product category to be deleted.</param>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task DeleteAsync(int id);
}
