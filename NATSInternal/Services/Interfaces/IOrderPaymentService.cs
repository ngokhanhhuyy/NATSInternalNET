namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Interface for Order Payment Service operations.
/// </summary>
public interface IOrderPaymentService
{
    /// <summary>
    /// Retrieves the details of a specific order payment by its ID.
    /// </summary>
    /// <param name="id">The ID of the order payment to retrieve.</param>
    /// <returns>
    /// A Task representing the asynchronous operation, with a result of OrderPaymentResponseDto.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the order payment with the specified ID is not found.
    /// </exception>
    Task<OrderPaymentResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new order payment.
    /// </summary>
    /// <param name="orderId">
    /// The ID of the order.
    /// </param>
    /// <param name="requestDto">The details of the order payment.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the response DTO with created payment details.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the order is not found.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when the order is closed or payment initialization fails.
    /// </exception>
    Task<OrderPaymentCreateResponseDto> CreateAsync(
            int orderId,
            OrderPaymentRequestDto requestDto);

    /// <summary>
    /// Creates a new order payment for a given order.
    /// </summary>
    /// <param name="order">The order entity.</param>
    /// <param name="requestDto">The details of the order payment.</param>
    /// <param name="propertyPathPrefix">The prefix of the property paths in the thrown exception.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the created order payment entity.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when the order is closed or payment initialization fails.
    /// </exception>
    Task<OrderPayment> CreateAsync(
            Order order,
            OrderPaymentRequestDto requestDto,
            string propertyPathPrefix);

    /// <summary>
    /// Updates an existing order payment.
    /// </summary>
    /// <param name="id">
    /// The ID of the order payment to update.
    /// </param>
    /// <param name="requestDto">The updated details of the order payment.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the order payment is not found.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when the order payment is closed or update fails.
    /// </exception>
    Task UpdateAsync(int id, OrderPaymentRequestDto requestDto);

    /// <summary>
    /// Deletes an existing order payment.
    /// </summary>
    /// <param name="id">
    /// The ID of the order payment to delete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the order payment is not found.
    /// </exception>
    /// <exception cref="OperationException">
    /// Thrown when the order payment or the related order is closed or deletion fails.
    /// </exception>
    Task DeleteAsync(int id);
}