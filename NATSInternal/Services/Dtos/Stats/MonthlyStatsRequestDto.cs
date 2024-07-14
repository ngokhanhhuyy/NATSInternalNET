namespace NATSInternal.Services.Dtos;

public class MonthlyStatsRequestDto : IRequestDto<MonthlyStatsRequestDto>
{
    public int RecordedMonth { get; set; }
    public int RecordedYear { get; set; }

    public MonthlyStatsRequestDto TransformValues()
    {
        return this;
    }
}