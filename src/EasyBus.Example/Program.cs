using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.DependencyInjection;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.Core.Publishing;
using EasyBus.Example;
using EasyBus.Inbox.AzureServiceBus;
using EasyBus.Inbox.Core;
using EasyBus.Inbox.Infrastructure;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.InMemory.DependencyInjection;
using EasyBus.Outbox.Core;
using EasyBux.Outbox.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

services.AddMessageQueue(config =>
{
    config.AddAzureServiceBus("Asd", opt => configuration.GetSection("Mq:AzureServiceBus:Asd").Bind(opt));
    config.AddInMemoryMq();

    config.AddPublisher(pub =>
    {
        //IF PROD
        pub.AddAzureServiceBusEventPublisher<SomeEvent>(
            "Asd",
            "my-topic",
            messageInterceptor: (@event, message) =>
            {
                message.CorrelationId = @event.ToString();
                message.PartitionKey = @event.ToString();
            });

        //IF LOCAL
        pub.AddInMemoryEventPublisher<SomeEvent>();

        pub.AddOutboxPublisher();
        pub.AddOutboxMessagesProcessor();
    });

    config.AddReceiver(rec =>
    {
        rec.AddInboxMessageConsumer();
        
        //IF PROD
        rec.AddAzureServiceBusTopicReceiver<SomeEvent>("Asd", "topic", "subscription")
            .SetFuncHandler(HandleAnyMessage)
            .SetInbox()
            .SetInboxFuncHandler((sp, mess, ct) =>
            {
                sp.GetRequiredService<ILogger>().LogInformation("Got {@Message}", mess);
                return Task.FromResult(InboxMessageState.NotReceived);
            });
        rec.AddAzureServiceBusQueueReceiver<SomeEvent>("Asd", "queue")
            .SetFuncHandler(HandleAnyMessage)
            .SetHandler<SomeEventReceiver>();

        //IF LOCAL
        rec.AddInMemoryReceiver<SomeEvent>()
            .SetFuncHandler(HandleAnyMessage);
    });
});

await using var sp = services.BuildServiceProvider();
var publisher = sp.GetRequiredService<IPublisher>();
var outboxPublisher = sp.GetRequiredService<IOutboxPublisher>();

await publisher.Publish(new SomeEvent(), CancellationToken.None);
await outboxPublisher.Publish(new SomeEvent(), CancellationToken.None);

static Task<bool> HandleAnyMessage<T>(IServiceProvider sp, T @event)
{
    sp.GetRequiredService<ILogger>().LogInformation("Received: {@Event}", @event);
    return Task.FromResult(true);
}

namespace EasyBus.Example
{
    public sealed record SomeEvent;

    public sealed class SomeEventReceiver : IAzureServiceBusMessageHandler<SomeEvent>
    {
        public Task MessageHandler(ProcessMessageEventArgs args, SomeEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}