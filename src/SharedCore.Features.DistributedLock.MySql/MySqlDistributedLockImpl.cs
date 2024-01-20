using Medallion.Threading.MySql;
using Microsoft.Extensions.Options;
using SharedCore.Features.DistributedLock.Core;

namespace SharedCore.Features.DistributedLock.MySql;

public sealed class MySqlDistributedLockImpl : IDistributedLock
{
    private readonly MySqlDistributedLockOptions _options;

    public MySqlDistributedLockImpl(IOptions<MySqlDistributedLockOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task CreateLock(string lockName, Func<Task> onLocked)
    {
        var @lock = new MySqlDistributedLock(lockName, _options.ConnectionString);
        await using (await @lock.AcquireAsync())
        {
            await onLocked();
        }
    }
}