namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle orders related operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Retrieves a list of orders based on the specified filtering and paginating conditions.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="OrderListRequestDto"/>, containing the filtering and
    /// paginating conditions for the results.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="OrderListResponseDto"/> class, containing the results and
    /// some additional information for pagination.
    /// </returns>
    Task<OrderListResponseDto> GetListAsync(OrderListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific order by its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the order to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="OrderDetailResponseDto"/> class, contaning the detailed
    /// information of the order.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the order with the specified id doesn't exist or has already been deleted.
    /// </exception>
    Task<OrderDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new order based on the specified request data.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="OrderUpsertRequestDto"/> class, containing the data for
    /// the new order.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// <see cref="int"/>, representing the id of the new order.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to perform the
    /// operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the customer with the id specified by the property <c>CustomerId</c> in
    /// the <c>requestDto</c> doesn't exist or has already been deleted.
    /// </exception>
    Task<int> CreateAsync(OrderUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing order based on the specified request data.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the order to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="OrderUpsertRequestDto"/>, containing the data for the
    /// order to be updated.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the order with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to perform the
    /// operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the customer with the id specified by the property <c>CustomerId</c> in
    /// the argument for the <c>requestDto</c> parameter doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task UpdateAsync(int id, OrderUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes a specific order by its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the order to be deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the order with the specified id doesn't exist or has already been deleted.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Throws when the requesting user doesn't have enough permissions to perform the
    /// operation.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    /// <exception cref="OperationException">
    /// Throws when the order's deletion is restricted due to the existence of some related
    /// data.
    /// </exception>
    Task DeleteAsync(int id);
}