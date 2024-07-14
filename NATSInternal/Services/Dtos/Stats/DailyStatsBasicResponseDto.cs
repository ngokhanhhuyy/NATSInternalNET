namespace NATSInternal.Services.Dtos;

public class DailyStatsBasicResponseDto : StatsBasicResponseDto
{
    public DateOnly RecordedDate { get; set; }
}