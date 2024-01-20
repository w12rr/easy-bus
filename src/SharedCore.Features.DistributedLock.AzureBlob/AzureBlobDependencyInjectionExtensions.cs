using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Features.DistributedLock.Core;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.AzureBlob;

public static class AzureBlobDependencyInjectionExtensions
{
    public static void AddAzureBlobDistributedLock(this IServiceCollection services)//, IConfigurationSection configuration)
    {
        // services.AddValidatedOptions<AzureBlobDistributedLockOptions, AzureBlobDistributedLockOptionsValidator>(configuration);
        services.AddScoped<IDistributedLock, AzureBlobDistributedLock>();
    }
}