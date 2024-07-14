namespace NATSInternal.Services.Dtos;

public class DailyStatsDetailResponseDto : StatsDetailResponseDto
{
    public DateOnly RecordedDate { get; set; }
}