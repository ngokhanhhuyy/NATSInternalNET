namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Service to handle debt incurrences.
/// </summary>
public interface IDebtIncurrenceService
{
    /// <summary>
    /// Retrieves the details of a specific debt incurrence by its ID.
    /// </summary>
    /// <param name="customerId">
    /// The ID of customer to which the retreiving debt incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// The ID of the debt incurrence to retrieve.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task
    /// result contains the debt incurrence detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer or the debt incurrence with the given id doesn't exist.
    /// </exception>
    Task<DebtIncurrenceDetailResponseDto> GetDetailAsync(int customerId, int debtIncurrenceId);
    
    /// <summary>
    /// Creates a new debt incurrence based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the creating debt incurrence belongs.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the details for the new debt incurrence.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the ID of the newly created debt incurrence.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Thrown when the current user doesn't have enough permission to
    /// specify some of the new entity's properties.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer with the specified id doesn't exist.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    Task<int> CreateAsync(int customerId, DebtIncurrenceUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt incurrence by its ID based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the updating debt incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// The ID of the debt incurrence to update.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the updated details for the debt incurrence.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer or the debt incurrence with the specified id doesn't
    /// exist or has been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the current user doesn't have enough permission to
    /// specify some of the new entity's properties.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is concurrent conflict during the operation.
    /// </exception>
    Task UpdateAsync(int customerId, int debtIncurrenceId,
        DebtIncurrenceUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt incurrence by its ID.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the debt incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// The ID of the debt incurrence to delete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer or the debt incurrence with the specified id
    /// doesn't exist or has been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user doesn't have permission to delete the specified
    /// debt incurrence.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is some concurrent conflict during the operation.
    /// </exception>
    Task DeleteAsync(int customerId, int debtIncurrenceId);
}