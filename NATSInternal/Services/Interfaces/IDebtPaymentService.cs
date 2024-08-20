namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Service to handle debt payments.
/// </summary>
public interface IDebtPaymentService
{
    /// <summary>
    /// Retrieves the details of a specific debt payment by its ID.
    /// </summary>
    /// <param name="customerId">
    /// The ID of customer to which the retrieving debt payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// The ID of the debt payment to retrieve.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the debt payment detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt payment with the given id doesn't exist.
    /// </exception>
    Task<DebtPaymentDetailResponseDto> GetDetailAsync(int customerId, int debtPaymentId);

    /// <summary>
    /// Creates a new debt payment based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the creating debt payment belongs.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the details for the new debt payment.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the ID of the newly created debt payment.
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
    Task<int> CreateAsync(int customerId, DebtPaymentUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt payment by its ID based on the specified request DTO.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the updating debt payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// The ID of the debt payment to update.
    /// </param>
    /// <param name="requestDto">
    /// The request DTO containing the updated details for the debt payment.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the debt payment with the given id doesn't exist or has been deleted.
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
    Task UpdateAsync(int customerId, int debtPaymentId, DebtPaymentUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt payment by its ID.
    /// </summary>
    /// <param name="customerId">
    /// The ID of the customer to which the deleting debt payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// The ID of the debt payment to delete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer or the debt payment with the specified id doesn't exist
    /// or has been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the current user doesn't have enough permission to
    /// delete the specified debt payment.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when there is some business logic violation during the operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is concurrent conflict during the operation.
    /// </exception>
    Task DeleteAsync(int customerId, int debtPaymentId);
}