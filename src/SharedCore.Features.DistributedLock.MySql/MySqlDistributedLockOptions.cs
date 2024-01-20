namespace SharedCore.Features.DistributedLock.MySql;

public sealed class MySqlDistributedLockOptions
{
    public required string ConnectionString { get; init; }
}