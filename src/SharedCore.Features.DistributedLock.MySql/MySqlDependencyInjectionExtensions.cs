using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Features.DistributedLock.Core;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.MySql;

public static class MySqlDependencyInjectionExtensions
{
    public static void AddMySqlDistributedLock(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddValidatedOptions<MySqlDistributedLockOptions, MySqlDistributedLockOptionsValidator>(configuration);
        services.AddScoped<IDistributedLock, MySqlDistributedLockImpl>();
    }
}