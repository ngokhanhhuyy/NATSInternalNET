namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle the product-related operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieve a list of products with the basic information, based on the specified
    /// filtering, sorting and paginating conditions.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="ProductListRequestDto"/> class, containing the conditions
    /// for the results.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="ProductListResponseDto"/> class, containing the results and
    /// the additional information for pagination.
    /// </returns>
    Task<ProductListResponseDto> GetListAsync(ProductListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific product, specified by its id.
    /// </summary>
    /// <param name="id">
    /// A <see cref="int"/> value representing the id of the product to retrieve.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="ProductDetailRequestDto"/>, containing the maximum
    /// result count for each of the <c>RecentSupplies</c>, <c>RecentOrders</c> and
    /// <c>RecentTreatments</c>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="ProductDetailResponseDto"/> class, containing the details
    /// of the product.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the product with the specified <c>id</c> doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task<ProductDetailResponseDto> GetDetailAsync(int id, ProductDetailRequestDto requestDto);

    /// <summary>
    /// Creates a new product based on the specified data.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="ProductUpsertRequestDto"/> class, contanining the data
    /// for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// <see cref="int"/> representing the id of the new product.
    /// </returns>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the brand with the id specified by the value of the property <c>BrandId</c>
    /// in the <c>requestDto</c> argument doesn't exist or has already been deleted.<br/>
    /// - When the category with the id specified by the value of the property
    /// <c>CategoryId</c> in the <c>requestDto</c> argument doens't exist or has already been
    /// deleted.<br/>
    /// - When the specfied value for the property <c>Name</c> in the <c>requestDto</c>
    /// argument already exists.
    /// </exception>
    Task<int> CreateAsync(ProductUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing product based on its id and the specified data.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the product to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="ProductUpsertRequestDto"/> class, containing the data
    /// for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the product with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the brand with the id specified by the value of the property <c>BrandId</c>
    /// in the <c>requestDto</c> argument doesn't exist or has already been deleted.<br/>
    /// - When the category with the id specified by the value of the property
    /// <c>CategoryId</c> in the <c>requestDto</c> argument doens't exist or has already been
    /// deleted.<br/>
    /// - When the specfied value for the property <c>Name</c> property in the
    /// <c>requestDto</c> argument already exists.
    /// </exception>
    Task UpdateAsync(int id, ProductUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes an existing product based on its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the product to delete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the product with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the product's deletion is restricted due to the existence of some related
    /// data.
    /// </exception>
    Task DeleteAsync(int id);
}
