namespace SharedCore.Features.DistributedLock.SqlServer;

public sealed class SqlServerDistributedLockOptions
{
    public required string ConnectionString { get; set; }
}