namespace NATSInternal.Services.Tasks;

public class StatsTaskService : IStatsTaskService
{
    public DateTime StartedDateTime { get; private set; }
    public DateTime ExpectedRestartingDateTime => StartedDateTime.AddHours(29);
    public TimeSpan RunningTime => DateTime.Now - StartedDateTime;
    public TimeSpan RemainingTime => ExpectedRestartingDateTime - DateTime.Now;

    public StatsTaskService()
    {
        StartedDateTime = DateTime.Now;
    }
}
