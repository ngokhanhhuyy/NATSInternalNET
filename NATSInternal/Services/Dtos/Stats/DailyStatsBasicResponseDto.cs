namespace NATSInternal.Services.Dtos;

public class DailyStatsBasicResponseDto : StatsBasicResponseDto
{
    public DateOnly RecordedDate { get; set; }

    public DailyStatsBasicResponseDto(DailyStats dailyStats)
    {
        Cost = dailyStats.Cost;
        Expenses = dailyStats.Expenses;
        GrossRevenue = dailyStats.GrossRevenue;
        NetRevenue = dailyStats.NetRevenue;
        NetProfit = dailyStats.NetProfit;
        IsTemporarilyClosed = dailyStats.TemporarilyClosedDateTime.HasValue;
        IsOfficiallyClosed = dailyStats.OfficiallyClosedDateTime.HasValue;
        RecordedDate = dailyStats.RecordedDate;
    }
}