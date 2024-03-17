using EasyBus.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Infrastructure;

public sealed class InboxConfiguration
{
    public IServiceCollection Services { get; set; }

    public InboxConfiguration(IServiceCollection services)
    {
        Services = services;
    }
    
    public void UseNamedConnectionString(string connectionStringName)
    {
        Services.AddOptions<InboxConnectionStringOptions>().Configure<IServiceProvider>((x, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            x.ConnectionString = configuration.GetConnectionString(connectionStringName).AssertNull();
        });

    }
    public void UseConnectionString(string connectionString)
    {
        Services.AddOptions<InboxConnectionStringOptions>().Configure(x => x.ConnectionString = connectionString);
    }
}