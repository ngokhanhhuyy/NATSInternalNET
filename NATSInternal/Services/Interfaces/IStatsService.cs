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
    Task IncrementRetailGrossRevenueAsync(long value, DateOnly? date = null);

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
    /// This method updates both the daily and monthly retail
    /// revenue statistics. If <paramref name="date"/> is not
    /// specified, the statistics for today are updated. The
    /// changes are persisted to the database immediately after
    /// the increment operation.
    /// </remarks>
    Task IncrementTreatmentGrossRevenueAsync(long value, DateOnly? date = null);

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
    /// This method updates both the daily and monthly consultant
    /// revenue statistics. If <paramref name="date"/> is not
    /// specified, the statistics for today are updated. The
    /// changes are persisted to the database immediately after
    /// the increment operation.
    /// </remarks>
    Task IncrementConsultantGrossRevenueAsync(long value, DateOnly? date = null);
    
    /// <summary>
    /// Increases the debt amount statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the debt amount.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly debt
    /// amount statistics. If <paramref name="date"/> is not
    /// specified, the statistics for today are updated. The
    /// changes are persisted to the database immediately
    /// after the increment operation.
    /// </remarks>
    Task IncrementDebtAmountAsync(long value, DateOnly? date = null);

    /// <summary>
    /// Increases the debt paid amount statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the debt paid amount.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly debt paid
    /// amount statistics. If <paramref name="date"/> is not specified,
    /// the statistics for today are updated. The changes are persisted
    /// to the database immediately after the increment operation.
    /// </remarks>
    Task IncrementDebtPaidAmountAsync(long amount, DateOnly? date = null);

    /// <summary>
    /// Increases the vat collected amount statistics for a specific date 
    /// or today if not specified.
    /// </summary>
    /// <param name="value">
    /// The amount by which to increment the vat collected amount.
    /// </param>
    /// <param name="date">
    /// Optional. The date for which to update the statistics. 
    /// If not provided, today's date is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method updates both the daily and monthly vat collected
    /// amount statistics. If <paramref name="date"/> is not
    /// specified, the statistics for today are updated. The changes
    /// are persisted to the database immediately after the increment
    /// operation.
    /// </remarks>
    Task IncrementVatCollectedAmountAsync(long amount, DateOnly? date = null);

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
    /// This method updates both the daily and monthly shipment
    /// cost statistics. If <paramref name="date"/> is not specified,
    /// the statistics for today are updated. The changes are
    /// persisted to the database immediately after the increment
    /// operation.
    /// </remarks>
    Task IncrementShipmentCostAsync(long value, DateOnly? date = null);
    
    /// <summary>
    /// Increases the expense with given category statistics
    /// for a specific date or today if not specified.
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
    /// This method updates both the daily and monthly expense
    /// statistics. If <paramref name="date"/> is not specified,
    /// the statistics for today are updated. The changes are
    /// persisted to the database immediately after the increment
    /// operation.
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
    
    /// <summary>
    /// Close daily stats by given date. This will write the datetime value when this method is called
    /// to the daily stats <c>TemporarilyClosedDateTime</c> proeprty.
    /// </summary>
    /// <param name="date">The date of daily stats which should be closed.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task TemporarilyCloseAsync(DateOnly date);

    /// <summary>
    /// Verify the datetime of the resource to be created (OrderedDateTime, CreatedDateTime,
    /// PaidDateTime, ...). The datetime must be later than the minimum opened datetime so
    /// that the resource will not be considered
    /// as closed.
    /// </summary>
    /// <param name="dateTime">The datetime of the new resource.</param>
    /// <returns>
    /// <c>True</c> if the datetime is later than the minimum opened datetime. Otherwise, <c>False</c>.
    /// </returns>
    bool VerifyResourceDateTimeToBeCreated(DateTime dateTime);

    /// <summary>
    /// Verify the datetime of the resource to be updated, based on the original datetime of
    /// the resource (OrderedDateTime, CreatedDateTime, PaidDateTime, ...). If the original datetime
    /// is earlier than the minimum opened datetime and considered closed, the new datetime must be
    /// also earler than the minimum opened datetime and vice versa. Both of them must be in the same
    /// status.
    /// </summary>
    /// <param name="originalDateTime"></param>
    /// <param name="newDateTime"></param>
    /// <returns></returns>
    bool VerifyResourceDateTimeToBeUpdated(DateTime originalDateTime, DateTime newDateTime);

    /// <summary>
    /// Get the minimum datetime that if a resource is assigned to, it will be considered <c>opened</c>.
    /// If it earlier than the current month's 4th 02:00, the minimum datetime will be 00:00 of the
    /// first day of 2 months ago. Otherwise, it will be the 00:00 of the first day of the last month.
    /// </summary>
    /// <returns>
    /// The minimum datetime that if a resource is assigned to, it will be considered <c>opened</c>.
    /// </returns>
    DateTime GetResourceMinimumOpenedDateTime();
}
