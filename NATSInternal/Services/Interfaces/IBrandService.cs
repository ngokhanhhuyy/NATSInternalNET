namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to perform the brand-related operations.
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Retrives a list which contains the basic information of brands based on the filtering
    /// and paginating conditions.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="BrandListResponseDto"/> class, containing the results.
    /// </returns>
    Task<BrandListResponseDto> GetListAsync();

    /// <summary>
    /// Retrives the details of a specific brand.
    /// </summary>
    /// <param name="id">
    /// A <see cref="int"/> represening the id of the brand.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="BrandDetailResponseDto"/>, containing the details of the
    /// brand.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the brand with the specified id doesn't exist or has already been deleted.
    /// </exception>
    Task<BrandDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a new brand with the data provided in the request.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="BrandRequestDto"/>, containing the data for the new
    /// brand.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// <see cref="int"/> representing the id of the new brand.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws when the country with the id specified by the value of the property
    /// <c>Country.Id</c> in the argument for the <c>requestDto</c> parameter doesn't exist or
    /// when the name specified by the value of the property <c>Name</c> in the argument for
    /// the <c>requestDto</c> parameter already exists.
    /// </exception>
    Task<int> CreateAsync(BrandRequestDto requestDto);

    /// <summary>
    /// Update an existing brand.
    /// </summary>
    /// <param name="id">
    /// A <see cref="int"/> representing the id of the brand to be updated.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="BrandRequestDto"/>, containing the data for the brand
    /// to be updated.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the brand with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the country with the id specified by the value of the property
    /// <c>Country.Id</c> doesn't exist or when the specified name by the property <c>Name</c>
    /// in the argument for the <c>requestDto</c> parameter already exists.
    /// </exception>
    Task UpdateAsync(int id, BrandRequestDto requestDto);

    /// <summary>
    /// Delete an existing brand.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the brand to be deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the brand with the specified id doesn't exist or has already been deleted.
    /// </exception>
    Task DeleteAsync(int id);
}
