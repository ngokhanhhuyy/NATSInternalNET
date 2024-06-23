namespace NATSInternal.Services.Interfaces;

/// <summary>
/// Provides methods to increment various financial metrics
/// and statistics for retail, treatment, consultant revenue,
/// shipment costs, and supply costs.
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Increases the retail revenue statistics for a specific date
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the retail revenue.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics.
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method updates both the daily and monthly retail revenue
    /// statistics. If <paramref name="date"/> is not specified, the
    /// statistics for today are updated. The changes are persisted
    /// to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementRetailRevenueAsync(long value, DateOnly? date = null);

    /// <summary>
    /// Increases the retail revenue statistics for a specific date or today 
    /// if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the retail revenue.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly retail revenue statistics.
    /// If <paramref name="date"/> is not specified, the statistics for today are updated.
    /// The changes are persisted to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementTreatmentRevenueAsync(long value, DateOnly? date = null);

    /// <summary>
    /// Increases the consultant revenue statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the consultant revenue.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly consultant revenue statistics.
    /// If <paramref name="date"/> is not specified, the statistics for today are updated.
    /// The changes are persisted to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementConsultantRevenueAsync(long value, DateOnly? date = null);

    /// <summary>
    /// Increases the shipment cost statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the shipment cost.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly shipment cost statistics.
    /// If <paramref name="date"/> is not specified, the statistics for today are updated.
    /// The changes are persisted to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementShipmentCostAsync(long value, DateOnly? date = null);
    
    /// <summary>
    /// Increases the expense with given category statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the expense.
    /// </param>
    /// <param name="category">
    /// The category of expense to increment.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly expense statistics.
    /// If <paramref name="date"/> is not specified, the statistics for today are updated.
    /// The changes are persisted to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementExpenseAsync(long value, ExpenseCategory category, DateOnly? date = null);

    /// <summary>
    /// Increases the supply cost statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the supply cost.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly 
    /// supply cost statistics. If <paramref name="date"/> 
    /// is not specified, the statistics for today are updated.
    /// The changes are persisted to the database 
    /// immediately after the increment operation.
    /// </remarks>
    Task IncrementSupplyCostAsync(long value, DateOnly? date = null);

}
