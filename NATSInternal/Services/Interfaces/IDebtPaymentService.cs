namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle the operations which are related to debt payment.
/// </summary>
public interface IDebtPaymentService
{
    /// <summary>
    /// Retrieves the details of a specific debt payment by its id.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of customer to which the retrieving debt
    /// payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// An <see cref="int"/> value representing the id of the debt payment to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="DebtPaymentDetailResponseDto"/> class, containing the
    /// details of the debt payment.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the debt payment specified by the <c>customerId</c> and the
    /// <c>debtPaymentId</c> arguments doesn't exist or has already been deleted.
    /// </exception>
    Task<DebtPaymentDetailResponseDto> GetDetailAsync(int customerId, int debtPaymentId);

    /// <summary>
    /// Creates a new debt payment for a specific customer, specfied by the id of the customer
    /// and the provided data.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of the customer to which the new debt
    /// payment belongs.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="DebtPaymentUpsertRequestDto"/> class, containing the data
    /// for the creating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asychronous operation.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to specify a value
    /// for the <c>PaidDateTime</c> property in the <c>requestDto</c> argument.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when the information of the requesting user has already been deleted before the
    /// operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - The customer specified by the <c>customerId</c> argument doesn't exist or has already
    /// been deleted.
    /// - The remaining debt amount of the specified customer becomes negative after the
    /// operation.
    /// </exception>
    Task<int> CreateAsync(int customerId, DebtPaymentUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt payment, based on the id of the customer to which it belongs,
    /// its id and the provided data.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of the customer to which the updating
    /// debt payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// An <see cref="int"/> value representing the id of the debt payment to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="DebtPaymentUpsertRequestDto"/> class, containing the data
    /// for the updating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the debt payment specified by the <c>customerId</c> and the
    /// <c>debtPaymentId</c> argument doesn't exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws under the following circumstances:<br/>
    /// - When the requesting user doesn't have enough permissions to update the debt payment.
    /// - When the requesting user can update the debt payment, but doesn't have enough
    /// permissions to specify a value for the <c>PaidDateTime</c> property in the
    /// <c>requestDto</c> argument.
    /// </exception>
    /// <exception cref="ValidationException">
    /// Throws when the value of the <c>PaidDateTime</c> property in the <c>requestDto</c>
    /// argument is invalid.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the <c>PaidDateTime</c> property in the <c>requestDto</c> argument is specified
    /// a value while the debt payment has already been locked.
    /// - When the remaining debt amount of the specified customer becomes negative after the
    /// operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws under the following circumstances:<br/>
    /// - When the debt payment has been modified by another process before the operation.<br/>
    /// - When the information of the requesting user has been deleted by another process
    /// before the operation.
    /// </exception>
    Task UpdateAsync(
        int customerId,
        int debtPaymentId,
        DebtPaymentUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt payment, specified by its id.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of the customer to which the deleting
    /// debt payment belongs.
    /// </param>
    /// <param name="debtPaymentId">
    /// An <see cref="int"/> value representing the id of the debt payment to delete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the debt payment specified by the <c>customerId</c> and the
    /// <c>debtPaymentId</c> argument doesn't exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to delete the specified
    /// debt payment.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the debt payment is locked.
    /// - When the specified customer's remaining debt amount becomes negative after the
    /// operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    Task DeleteAsync(int customerId, int debtPaymentId);
}