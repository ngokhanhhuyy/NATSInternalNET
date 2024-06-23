namespace NATSInternal.Services.Tasks;

public class StatsTaskService : IStatsTaskService
{
    public DateTime StartedDateTime { get; private set; }
    public DateTime ExpectedRestartingDateTime => StartedDateTime.AddHours(29);
    public TimeSpan RunningTime => DateTime.UtcNow.ToApplicationTime() - StartedDateTime;
    public TimeSpan RemainingTime => ExpectedRestartingDateTime - DateTime.UtcNow.ToApplicationTime();

    public StatsTaskService()
    {
        StartedDateTime = DateTime.UtcNow.ToApplicationTime();
    }
}
