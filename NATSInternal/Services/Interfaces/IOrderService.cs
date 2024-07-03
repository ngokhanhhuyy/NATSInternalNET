namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Interface for order service that handles the operations related to orders.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Retrieves a list of orders based on the specified request criteria.
    /// </summary>
    /// <param name="requestDto">The request DTO containing filter, sort, and pagination information.</param>
    /// <returns>A Task representing the asynchronous operation, with a result of OrderListResponseDto.</returns>
    Task<OrderListResponseDto> GetListAsync(OrderListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>A Task representing the asynchronous operation, with a result of OrderDetailResponseDto.</returns>
    /// <exception cref="ResourceNotFoundException">Thrown when the order with the specified ID is not found.</exception>
    Task<OrderDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new order based on the specified request data.
    /// </summary>
    /// <param name="requestDto">The request DTO containing the data for the new order.</param>
    /// <returns>A Task representing the asynchronous operation, with a result of the created order's ID.</returns>
    /// <exception cref="OperationException">Thrown when there is an error during the creation of the order.</exception>
    Task<int> CreateAsync(OrderUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing order based on the specified request data.
    /// </summary>
    /// <param name="id">The ID of the order to update.</param>
    /// <param name="requestDto">The request DTO containing the updated data for the order.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">Thrown when the order with the specified ID is not found.</exception>
    /// <exception cref="AuthorizationException">Thrown when the current user does not have permission to edit the order.</exception>
    /// <exception cref="OperationException">Thrown when there is an error during the update of the order.</exception>
    Task UpdateAsync(int id, OrderUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">Thrown when the order with the specified ID is not found.</exception>
    /// <exception cref="AuthorizationException">Thrown when the current user does not have permission to delete the order.</exception>
    /// <exception cref="OperationException">Thrown when there is an error during the deletion of the order.</exception>
    Task DeleteAsync(int id);
}