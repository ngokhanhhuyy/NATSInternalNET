namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Service to handle debts.
/// </summary>
public interface IDebtService
{
    /// <summary>
    /// Retrieves a paginated list of debts based on the specified request criteria.
    /// </summary>
    /// <param name="requestDto">
    /// The request criteria for retrieving the debt list.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the debt list response DTO.
    /// </returns>
    Task<DebtListResponseDto> GetListAsync(DebtListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific debt by its ID.
    /// </summary>
    /// <param name="customerId">
    /// The ID of customer to which the retreiving debt belongs.
    /// </param>
    /// <param name="debtId">
    /// The ID of the debt to retrieve.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the debt detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt with the given id doesn't exist.
    /// </exception>
    Task<DebtDetailResponseDto> GetDetailAsync(int customerId, int debtId);
    
    /// <summary>
    /// Creates a new debt based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the creating debt belongs.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the details for the new debt.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the ID of the newly created debt.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Thrown when the current user doesn't have enough permission to
    /// specify some of the new entity's properties.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    Task<int> CreateAsync(int customerId, DebtUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt by its ID based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the updating debt belongs.
    /// </param>
    /// <param name="debtId">
    /// The ID of the debt to update.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the updated details for the debt.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt with the given id doesn't exist or has been deleted.
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
    Task UpdateAsync(int customerId, int debtId, DebtUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt by its ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to which the debt belongs.</param>
    /// <param name="debtId">The ID of the debt to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt with the given id doesn't exist or has been deleted.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is concurrent conflict during the operation.
    /// </exception>
    Task DeleteAsync(int customerId, int debtId);
}