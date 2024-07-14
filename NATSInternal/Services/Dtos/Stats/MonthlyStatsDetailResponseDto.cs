namespace NATSInternal.Services.Dtos;

public class MonthlyStatsDetailResponseDto : StatsDetailResponseDto
{
    public int RecordedYear { get; set; }
    public int RecordedMonth { get; set; }
    public List<DailyStatsBasicResponseDto> DailyStats { get; set; }
}