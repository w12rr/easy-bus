using FluentValidation;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.AzureBlob;

public class AzureBlobDistributedLockOptionsValidator : OptionsValidator<AzureBlobDistributedLockOptions>
{
    public AzureBlobDistributedLockOptionsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
        RuleFor(x => x.LockingContainerName).NotEmpty();
    }
}