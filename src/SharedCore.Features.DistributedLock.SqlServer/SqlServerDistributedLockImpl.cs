using Medallion.Threading.SqlServer;
using Microsoft.Extensions.Options;
using SharedCore.Features.DistributedLock.Core;

namespace SharedCore.Features.DistributedLock.SqlServer;

public sealed class SqlServerDistributedLockImpl : IDistributedLock
{
    private readonly SqlServerDistributedLockOptions _options;

    public SqlServerDistributedLockImpl(IOptions<SqlServerDistributedLockOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task CreateLock(string lockName, Func<Task> onLocked)
    {
        var @lock = new SqlDistributedLock(lockName, _options.ConnectionString);
        await using (await @lock.AcquireAsync())
        {
            await onLocked();
        }
    }
}