namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle treatments.
/// </summary>
public interface ITreatmentService
{
    /// <summary>
    /// Get a list of treatments which each treatment contains basic
    /// information with filtering condition.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing filtering condition for the results.
    /// </param>
    /// <returns>The list of treatments.</returns>
    Task<TreatmentListResponseDto> GetListAsync(TreatmentListRequestDto requestDto);

    /// <summary>
    /// Get the detail information of a treatment with the given id.
    /// </summary>
    /// <param name="id">The id of the treatment.</param>
    /// <returns>
    /// An object containing the detail information of the treatment.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the treatment with the given id doesn't exist in the datbase.
    /// </exception>
    Task<TreatmentDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a treatment using the data provided in the request.
    /// </summary>
    /// <param name="requestDto">A object containing the data for a new treatment.</param>
    /// <returns>The id of the created treatment.</returns>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user doesn't have enough permission to set one or some
    /// of the treatment properties..
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when the is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throw when there is a concurrency conflict during the operation.
    /// </exception>
    Task<int> CreateAsync(TreatmentUpsertRequestDto requestDto);

    /// <summary>
    /// Update a treatment with the given id using the data provided in the request.
    /// </summary>
    /// <param name="id">The id of the treatment to be updated.</param>
    /// <param name="requestDto">A object containing the new data to be updated.</param>
    /// <returns>A Task object representing the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the treatment with the given id doesn't exist in the datbase.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user doesn't have enough permission to set one or some
    /// of the treatment properties..
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when the is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throw when there is a concurrency conflict during the operation.
    /// </exception>
    Task UpdateAsync(int id, TreatmentUpsertRequestDto requestDto);

    /// <summary>
    /// Delete the treatment with the given id. If the treatment cannot be deleted due
    /// to the restriction deleting behavior, the treatment will be soft deleted instead.
    /// </summary>
    /// <param name="id">The id of the treatment to be deleted.</param>
    /// <returns>A Task object representing the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the treatment with the given id doesn't exist in the datbase.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user doesn't have enough permission to set one or some
    /// of the treatment properties..
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throw when there is a concurrency conflict during the operation.
    /// </exception>
    Task DeleteAsync(int id);
}
