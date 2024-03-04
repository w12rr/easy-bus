using EasyBus.Core.Publishing;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyBus.Outbox;

public static class OutboxDependencyInjectionExtensions
{
    public static void AddOutboxPublisher(this PublisherConfiguration config, string dbConnectionString)
    {
        config.Services.AddScoped<IOutboxPublisher>(sp =>
            new OutboxPublisher(dbConnectionString, sp.GetRequiredService<ILogger<OutboxPublisher>>()));
    }

    public static void AddOutboxMessagesProcessor(this PublisherConfiguration config, string dbConnectionString)
    {
        config.Services.AddHostedService<OutboxBackgroundService>(sp =>
            new OutboxBackgroundService(
                dbConnectionString,
                sp.GetRequiredService<ILogger<OutboxBackgroundService>>(),
                sp.GetRequiredService<IPublisher>()));
    }
}