namespace SharedCore.Features.DistributedLock.AzureBlob;

public sealed class AzureBlobDistributedLockOptions
{
    public required string ConnectionString { get; init; }
    public required string LockingContainerName { get; init; }
}