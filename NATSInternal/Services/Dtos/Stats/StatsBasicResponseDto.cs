namespace NATSInternal.Services.Dtos;

public class StatsBasicResponseDto
{
    public long Cost { get; set; }
    public long Expenses { get; set; }
    public long GrossRevenue { get; set; }
    public long NetRevenue { get; set; }
    public long NetProfit { get; set; }
    public bool IsTemporarilyClosed { get; set; }
    public bool IsOfficiallyClosed { get; set; }
}