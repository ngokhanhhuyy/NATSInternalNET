namespace NATSInternal.Services.Interfaces;

public interface ILockableEntity
{
    bool IsLocked { get; }
}