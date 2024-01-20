using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Features.DistributedLock.Core;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.SqlServer;

public static class SqlServerDependencyInjectionExtensions
{
    public static void AddSqlServerDistributedLock(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddValidatedOptions<SqlServerDistributedLockOptions, SqlServerDistributedLockOptionsValidator>(configuration);
        services.AddScoped<IDistributedLock, SqlServerDistributedLockImpl>();
    }
    public static void AddSqlServerDistributedLock(this IServiceCollection services, string connectionString)
    {
        services.AddValidatedOptions<SqlServerDistributedLockOptions, SqlServerDistributedLockOptionsValidator>();
        services.PostConfigure<SqlServerDistributedLockOptions>(x =>
        {
            x.ConnectionString = connectionString;
        });
        services.AddScoped<IDistributedLock, SqlServerDistributedLockImpl>();
    }
}