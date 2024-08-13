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
    /// Retrieves a paginated list of customers who have remaining debt based on the specified request criteria.
    /// </summary>
    /// <param name="requestDto">
    /// The request criteria for retrieving the debt list.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the list of customers with remaining debt amount response DTO.
    /// </returns>
    Task<DebtByCustomerListResponseDto> GetRemainingAmountListByCustomersAsync(
            DebtByCustomerListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific debt by its ID.
    /// </summary>
    /// <param name="id">The ID of the debt to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the debt detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt with the given id doesn't exist.
    /// </exception>
    Task<DebtDetailResponseDto> GetDetailAsync(int id);
    
    /// <summary>
    /// Creates a new debt based on the specified request DTO.
    /// </summary>
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
    Task<int> CreateAsync(DebtUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt by its ID based on the specified request DTO.
    /// </summary>
    /// <param name="id">
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
    Task UpdateAsync(int id, DebtUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt by its ID.
    /// </summary>
    /// <param name="id">The ID of the debt to delete.</param>
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
    Task DeleteAsync(int id);
}