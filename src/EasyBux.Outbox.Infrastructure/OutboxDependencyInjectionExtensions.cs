using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Outbox.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasyBux.Outbox.Infrastructure;

public static class OutboxDependencyInjectionExtensions
{
    public static void AddOutboxServices(this PublisherConfiguration config, Action<OutboxConfig> outboxConfigAction)
    {
        config.Services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        config.Services.AddScoped<IMissingTableCreator, MissingTableCreator>();
        config.Services.AddScoped<IOutboxFromDbToTransportWriter, OutboxFromDbToTransportWriter>();
        config.Services.AddTransient<IOutboxRepository, OutboxRepository>();
        outboxConfigAction(new OutboxConfig(config.Services));
        config.Services.AddHostedService<OutboxBackgroundService>();
    }
}