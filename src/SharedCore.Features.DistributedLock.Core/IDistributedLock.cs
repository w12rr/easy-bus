namespace SharedCore.Features.DistributedLock.Core;

public interface IDistributedLock
{
    Task CreateLock(string lockName, Func<Task> onLocked);
}