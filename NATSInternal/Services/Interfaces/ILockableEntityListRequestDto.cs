namespace NATSInternal.Services.Interfaces;

public interface ILockableEntityListRequestDto
{
    int Month { get; set; }
    int Year { get; set; }
    bool IgnoreMonthYear { get; set; }
}
