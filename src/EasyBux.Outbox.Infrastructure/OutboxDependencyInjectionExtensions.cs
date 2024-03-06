using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Outbox.Core;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBux.Outbox.Infrastructure;

public static class OutboxDependencyInjectionExtensions
{
    public static void AddOutboxPublisher(this PublisherConfiguration config)
    {
        config.Services.AddScoped<IOutboxPublisher, OutboxPublisher>();
    }

    public static void AddOutboxMessagesProcessor(this PublisherConfiguration config)
    {
        config.Services.AddHostedService<OutboxBackgroundService>();
    }
}