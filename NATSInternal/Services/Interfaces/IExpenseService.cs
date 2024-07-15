namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Interface for the ExpenseService, providing methods for managing expenses.
/// </summary>
public interface IExpenseService
{
    /// <summary>
    /// Gets a paginated list of expenses based on the specified request parameters.
    /// </summary>
    /// <param name="requestDto">
    /// The request parameters for fetching the expense list.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the expense list response DTO.
    /// </returns>
    Task<ExpenseListResponseDto> GetListAsync(ExpenseListRequestDto requestDto);

    /// <summary>
    /// Gets the detailed information of a specific expense by its ID.
    /// </summary>
    /// <param name="id">The ID of the expense.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the expense detail response DTO.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the expense with the specified ID is not found.
    /// </exception>
    Task<ExpenseDetailResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new expense with the specified data.
    /// </summary>
    /// <param name="requestDto">
    /// The data transfer object containing the details of the expense to create.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the ID of the created expense.</returns>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task<int> CreateAsync(ExpenseUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing expense with the specified data.
    /// </summary>
    /// <param name="id">
    /// The ID of the expense to update.
    /// </param>
    /// <param name="requestDto">
    /// The data transfer object containing the updated details of the expense.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the expense with the specified ID is not found.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// Thrown when the user is not authorized to edit the expense.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task UpdateAsync(int id, ExpenseUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes an existing expense by its ID.
    /// </summary>
    /// <param name="id">The ID of the expense to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the expense with the specified ID is not found.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown the operation faces concurrency conflict.
    /// </exception>
    Task DeleteAsync(int id);
}