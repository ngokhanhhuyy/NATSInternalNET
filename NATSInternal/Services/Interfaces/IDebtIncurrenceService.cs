namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle the operations which are related to debt incurrence.
/// </summary>
public interface IDebtIncurrenceService
{
    /// <summary>
    /// Retrieves the details of a specific debt incurrence by its id.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of the customer to which the retrieving
    /// debt incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// An <see cref="int"/> value representing the id of the debt incurrence to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="DebtIncurrenceDetailResponseDto"/>, containing the details
    /// of the debt incurrence.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the debt incurrence specified by the <c>customerId</c> and
    /// <c>debtIncurrenceId</c> arguments doesn't exist or has already been deleted.
    /// </exception>
    Task<DebtIncurrenceDetailResponseDto> GetDetailAsync(int customerId, int debtIncurrenceId);
    
    /// <summary>
    /// Creates a new debt incurrence based on the specified data.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> representing the id of the customer to which the retrieving debt
    /// incurrence belongs to.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="DebtIncurrenceUpsertRequestDto"/> class, containing the
    /// data for the creating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// <see cref="int"/> value representing the id of the new debt incurrence.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the customer which has the id specified by the value of the
    /// <c>customerId</c> argument doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doens't have enough permissions to specify a value for
    /// the <c>IncurredDateTime</c> property in the <c>requestDto</c> argument.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when information of the requesting user has been deleted before the operation.
    /// </exception>
    Task<int> CreateAsync(int customerId, DebtIncurrenceUpsertRequestDto requestDto);
    
    /// <summary>
    /// Updates an existing debt incurrence, based on the id of the customer to which it
    /// belongs, its id and the specified data.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> representing the id of the customer to which the updating debt
    /// incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// An <see cref="int"/> representing the id of the debt incurrence to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="DebtIncurrenceUpsertRequestDto"/> class, containing the
    /// data for the updating operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Throws when the specified value for the <c>IncurredDateTime</c> property is invalid.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the customer which has the id specified by the value of the `customerId`
    /// argument doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws under the following circumstances:<br/>
    /// - When the requesting user doesn't have enough permissions to update the specified debt
    /// incurrencesome of the new entity's properties.<br/>
    /// - When the requesting user doesn't have enough permissions to specify a value for the
    /// <c>IncurredDateTime</c> property in the <c>requestDto</c> argument.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws under the following circumstances:<br/>
    /// - When the information of the requesting user has been deleted before the operation.
    /// <br/>
    /// - When the debt incurrence information has been modified before the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - The <c>IncurredDateTime</c> property in the <c>requestDto</c> argument is specified a
    /// value when the debt incurrence has already been locked.<br/>
    /// - The remaining debt amount of the specified customer becomes negative after the
    /// operation.
    /// </exception>
    Task UpdateAsync(
        int customerId,
        int debtIncurrenceId,
        DebtIncurrenceUpsertRequestDto requestDto);
    
    /// <summary>
    /// Deletes an existing debt incurrence based on its id.
    /// </summary>
    /// <param name="customerId">
    /// An <see cref="int"/> value representing the id of the customer to which the debt
    /// incurrence belongs.
    /// </param>
    /// <param name="debtIncurrenceId">
    /// An <see cref="int"/> value representing the id of the debt incurrence to delete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the customer specified by the <c>customerId</c> argument, or the debt
    /// incurrence specified by the <c>debtIncurrenceId</c> argument doesn't exist or has been
    /// deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to delete the specified
    /// debt incurrence.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws under the following circumstances:<br/>
    /// - When the information of the requesting user has been deleted before the operation.
    /// <br/>
    /// - When the debt incurrence has been modified before the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the remaining debt amount of the specified customer becomes negative after
    /// the operation.
    /// </exception>
    Task DeleteAsync(int customerId, int debtIncurrenceId);
}