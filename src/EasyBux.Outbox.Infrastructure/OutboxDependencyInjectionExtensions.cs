using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Outbox.Core;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBux.Outbox.Infrastructure;

public static class OutboxDependencyInjectionExtensions
{
    public static void AddOutboxPublisher(this PublisherConfiguration config, string dbConnectionString)
    {
        config.Services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        config.Services.AddScoped<IMissingTableCreator, MissingTableCreator>();
        config.Services.AddScoped<IOutboxFromDbToTransportWriter, OutboxFromDbToTransportWriter>();
        config.Services.AddScoped<IOutboxRepository>(_ => new OutboxRepository(dbConnectionString));
    }

    public static void AddOutboxMessagesProcessor(this PublisherConfiguration config)
    {
        config.Services.AddHostedService<OutboxBackgroundService>();
    }
}