using EasyBus.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBux.Outbox.Infrastructure;

public sealed class OutboxConfig
{
    public IServiceCollection Services;

    public OutboxConfig(IServiceCollection services)
    {
        Services = services;
    }

    public void UseNamedConnectionString(string connectionStringName)
    {
        Services.AddOptions<OutboxConnectionStringOptions>().Configure<IServiceProvider>((x, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            x.ConnectionString = configuration.GetConnectionString(connectionStringName).AssertNull();
        });

    }
    public void UseConnectionString(string connectionString)
    {
        Services.AddOptions<OutboxConnectionStringOptions>().Configure(x => x.ConnectionString = connectionString);
    }
}