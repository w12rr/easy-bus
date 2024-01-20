using Azure.Storage.Blobs;
using Medallion.Threading.Azure;
using Microsoft.Extensions.Options;
using SharedCore.Features.DistributedLock.Core;

namespace SharedCore.Features.DistributedLock.AzureBlob;

public sealed class AzureBlobDistributedLock : IDistributedLock
{
    private readonly AzureBlobDistributedLockOptions _options;

    public AzureBlobDistributedLock(IOptions<AzureBlobDistributedLockOptions> options)
    {
        _options = options.Value;
    }

    public async Task CreateLock(string lockName, Func<Task> onLocked)
    {
        var container = new BlobContainerClient(_options.ConnectionString, _options.LockingContainerName);
        var @lock = new AzureBlobLeaseDistributedLock(container, lockName);
        await using var handle = await @lock.TryAcquireAsync();

        if (handle != null) await onLocked();
    }
}