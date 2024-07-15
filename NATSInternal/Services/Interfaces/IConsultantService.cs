namespace NATSInternal.Services.Dtos;

/// <summary>
/// Service to handle consultants.
/// </summary>
public interface IConsultantService
{
    /// <summary>
    /// Gets a paginated list of consultants based on the specified request parameters.
    /// </summary>
    /// <param name="requestDto">
    /// The request parameters for fetching the consultant list.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the consultant list response DTO.
    /// </returns>
    Task<ConsultantListResponseDto> GetListAsync(ConsultantListRequestDto requestDto);

    /// <summary>
    /// Gets the detailed information of a specific consultant by its ID.
    /// </summary>
    /// <param name="id">The ID of the consultant.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the consultant detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the consultant with the specified ID is not found.
    /// </exception>
    Task<ConsultantDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new consultant with the specified data.
    /// </summary>
    /// <param name="requestDto">
    /// The data transfer object containing the details of the consultant to create.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the ID of the created consultant.</returns>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user is not authorized to edit the consultant.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business violation during the operation.
    /// </execption>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task<int> CreateAsync(ConsultantUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing consultant with the specified data.
    /// </summary>
    /// <param name="id">
    /// The ID of the consultant to update.
    /// </param>
    /// <param name="requestDto">
    /// The data transfer object containing the updated details of the consultant.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the consultant with the specified ID is not found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user doens't have enough permissions to specify the value(s) for
    /// one or many of the consultant properties..
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business violation during the operation.
    /// </execption>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task UpdateAsync(int id, ConsultantUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes an existing consultant by its ID.
    /// </summary>
    /// <param name="id">The ID of the consultant to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the consultant with the specified ID is not found.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there are some other related entities which restrict
    /// the entity to be deleted.
    /// </execption>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task DeleteAsync(int id);
}