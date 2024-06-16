namespace NATSInternal.Services;

/// <inheritdoc />
public class StatsService : IStatsService
{
    private readonly DatabaseContext _context;

    public StatsService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task IncrementRetailRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.RetailRevenue += value;
        dailyStats.Monthly.RetailRevenue += value;
        await _context.SaveChangesAsync();
    }
    
    public async Task IncrementTreatmentRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.TreatmentRevenue += value;
        dailyStats.Monthly.TreatmentRevenue += value;
        await _context.SaveChangesAsync();
    }

    public async Task IncrementConsultantRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.ConsultantRevenue += value;
        dailyStats.Monthly.ConsultantRevenue += value;
        await _context.SaveChangesAsync();
    }

    public async Task IncrementShipmentCostAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.ShipmentCost += value;
        dailyStats.Monthly.ShipmentCost += value;
        await _context.SaveChangesAsync();
    }

    public async Task IncrementSupplyCostAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.SupplyCost += value;
        dailyStats.Monthly.SupplyCost += value;
        await _context.SaveChangesAsync();
    }

    public async Task TemporarilyCloseAsync(DateOnly date)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.TemporarilyClosedDateTime = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    private async Task<DailyStats> FetchStatisticsEntitiesAsync(DateOnly? date = null)
    {
        DateOnly dateValue = date ?? DateOnly.FromDateTime(DateTime.Today);
        DailyStats dailyStats = await _context.DailyStats
            .Include(ds => ds.Monthly)
            .Where(ds => ds.RecordedDate == dateValue)
            .SingleOrDefaultAsync();

        if (dailyStats == null)
        {
            dailyStats = new DailyStats
            {
                RecordedDate = dateValue,
                CreatedDateTime = DateTime.Now,
            };
            _context.DailyStats.Add(dailyStats);

            MonthlyStats monthlyStats = await _context.MonthlyStats
                .Where(ms => ms.RecordedYear == dateValue.Year)
                .Where(ms => ms.RecordedMonth == dateValue.Month)
                .SingleOrDefaultAsync();
            if (monthlyStats == null)
            {
                monthlyStats = new MonthlyStats
                {
                    RecordedMonth = dateValue.Month,
                    RecordedYear = dateValue.Year
                };
                _context.MonthlyStats.Add(monthlyStats);
            }

            dailyStats.Monthly = monthlyStats;
            await _context.SaveChangesAsync();
        }

        return dailyStats;
    }
}
