namespace NATSInternal.Services.Interfaces;

public interface IStatsTaskService
{
    DateTime StartedDateTime { get; }
    DateTime ExpectedRestartingDateTime { get; }
    TimeSpan RunningTime { get; }
    TimeSpan RemainingTime { get; }
}
