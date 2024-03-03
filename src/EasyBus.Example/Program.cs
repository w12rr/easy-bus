using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.DependencyInjection;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.Core.Publishing;
using EasyBus.Example;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

services.AddMessageQueue(config =>
{
    config.AddAzureServiceBus("Asd", opt => configuration.GetSection("Mq:AzureServiceBus:Asd").Bind(opt));

    config.AddPublisher(pub =>
    {
        pub.AddAzureServiceBusEventPublisher<SomeEvent>("Asd", "my-topic");
        // pub.AddOutboxStore<AppDbContext>();
        // pub.AddOutboxMessageProducer<AppDbContext>();
    });

    config.AddReceiver(rec =>
    {
        rec.AddAzureServiceBusTopicReceiver<SomeEvent, SomeEventReceiver>("Asd", "topic", "subscription");
        rec.AddAzureServiceBusQueueReceiver<SomeEvent, SomeEventReceiver>("Asd", "queue");
        // rec.AddInboxStore<AppDbContext>();
        // rec.AddInboxMessageConsumer<AppDbContext>();
    });
});

await using var sp = services.BuildServiceProvider();
var publisher = sp.GetRequiredService<IPublisher>();

await publisher.Publish(new SomeEvent(), CancellationToken.None);

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