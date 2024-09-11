namespace NATSInternal.Services.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Get a list of products ased on condition.
    /// </summary>
    /// <param name="requestDto">An object containing filter conditions.</param>
    /// <returns>A list containing products' basic information</returns>
    Task<ProductListResponseDto> GetListAsync(ProductListRequestDto requestDto);

    /// <summary>
    /// Get detail information of the product with given id.
    /// </summary>
    /// <param name="id">The id of the product.</param>
    /// <param name="requestDto">
    /// An instance of the <see cref="ProductDetailRequestDto"/>, containing the maximum
    /// result counts for related <c>RecentSupplies</c>, <c>RecentOrders</c> and
    /// <c>RecentTreatments</c>.
    /// </param>
    /// <returns>An object containing detail information of the product.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task<ProductDetailResponseDto> GetDetailAsync(int id, ProductDetailRequestDto requestDto);

    /// <summary>
    /// Create a new product with given data from request.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing the data for the product to be created.
    /// </param>
    /// <returns>The id of the created product.</returns>
    /// <exception cref="OperationException"></exception>
    Task<int> CreateAsync(ProductUpsertRequestDto requestDto);

    /// <summary>
    /// Update the product which has given id with the new data.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing the data for the product to be updated.
    /// </param>
    /// <exception cref="OperationException"></exception>
    Task UpdateAsync(int id, ProductUpsertRequestDto requestDto);

    /// <summary>
    /// Delete the product with given id.
    /// </summary>
    /// <param name="id">The id of the product to be deleted.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    Task DeleteAsync(int id);
}
