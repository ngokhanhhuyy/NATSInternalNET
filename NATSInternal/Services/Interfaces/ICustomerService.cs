namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle customers.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Get a list of customers with pagination, filtering and sorting options.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing all the options for the list results.
    /// </param>
    /// <returns>
    /// An object containing the results and page count (for pagination calculation).
    /// </returns>
    Task<CustomerListResponseDto> GetListAsync(CustomerListRequestDto requestDto);

    /// <summary>
    /// Get basic information of the customer with given id.
    /// </summary>
    /// <param name="id">The id of the customer</param>
    /// <returns>
    /// An object containing the basic information of the customer.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// The customer with the given id doesn't exist in the database.
    /// </exception>
    Task<CustomerBasicResponseDto> GetBasicAsync(int id);

    /// <summary>
    /// Get fully detailed information of the customer with given id.
    /// </summary>
    /// <param name="id">The id of the customer.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The customer with the given id doesn't exist in the database.
    /// </exception>
    Task<CustomerDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a customer with provided data.
    /// </summary>
    /// <param name="requestDto">An object containing data for a new customer.</param>
    /// <returns>An object containing the id of the created customer.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// The introducer (customer) with the given id doesn't exist in the database.
    /// </exception>
    Task<int> CreateAsync(CustomerUpsertRequestDto requestDto);

    /// <summary>
    /// Update a customer who has the given id with the provided data.
    /// </summary>
    /// <param name="id">The id of the customer to be updated.</param>
    /// <param name="requestDto">
    /// An object containing new data for the customer to be updated.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The customer or the introducer with given id doesn't exist in the database.
    /// </exception>
    Task UpdateAsync(int id, CustomerUpsertRequestDto requestDto);

    /// <summary>
    /// Delete a customer who has given id.
    /// </summary>
    /// <param name="id">The id of the customer to be deleted.</param>
    /// <returns></returns>
    /// <exception cref="ResourceNotFoundException">
    /// The customer with given id doesn't exist in the database.
    /// </exception>
    Task DeleteAsync(int id);
}