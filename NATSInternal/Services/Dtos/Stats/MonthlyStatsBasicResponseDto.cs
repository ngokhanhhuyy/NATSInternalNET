namespace NATSInternal.Services.Dtos;

public class MonthlyStatsBasicResponseDto : StatsBasicResponseDto
{
    public int RecordedYear { get; set; }
    public int RecordedMonth { get; set; }
}