namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle treatments.
/// </summary>
public interface ITreatmentService
{
    /// <summary>
    /// Retrieves a list of treatments based on the specified filtering and paginating
    /// conditions.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="TreatmentListRequestDto"/> class, containing the
    /// filtering and paginating conditions for the results.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="TreatmentListResponseDto"/> class, containing the results
    /// and some additional information for pagination.
    /// </returns>
    Task<TreatmentListResponseDto> GetListAsync(TreatmentListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific treatment by its id.
    /// </summary>
    /// <param name="id">
    /// A <see cref="int"/> representing the id of the treatment to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="TreatmentDetailResponseDto"/> class, containing the details
    /// of the treatment.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the treatment with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task<TreatmentDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a new treatment based on the specified request data.
    /// </summary>
    /// <param name="requestDto">
    /// A instance of the <see cref="TreatmentUpsertRequestDto"/> class, containing the data
    /// for a new treatment.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// <see cref="int"/> representing the id of the new treatment.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Throws when the user doesn't have enough permission to set the value for one or some
    /// of the treatment properties.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the customer with the id specified by the value of the property
    /// <c>CustomerId</c> or the user with the id specified by the value of the property
    /// <c>TherapistId</c> in the argument for the <c>requestDto</c> parameter doesn't exist
    /// or has already been deleted.
    /// </exception>
    Task<int> CreateAsync(TreatmentUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing treatment with the specified request data.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the updating treatment.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="TreatmentUpsertRequestDto"/> class, containing the data
    /// for the treatment to be updated.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the treatment with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the user doesn't have enough permission to set the value for one or some
    /// of the treatment properties.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the customer with the id specified by the value of the property
    /// <c>CustomerId</c> or the user with the id specified by the value of the property
    /// <c>TherapistId</c> in the argument for the <c>requestDto</c> parameter doesn't exist
    /// or has already been deleted.
    /// </exception>
    Task UpdateAsync(int id, TreatmentUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes an existing treatment with the specified id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the deleting treatment.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// If the treatment cannot be deleted entirely due to the existence of some related data,
    /// it will be soft-deleted instead.
    /// </remarks>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the treatment with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the user doesn't have enough permissions to set the value for one or some
    /// of the treatment properties.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    Task DeleteAsync(int id);
}