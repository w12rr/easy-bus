using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.DependencyInjection;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.Core.Publishing;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

services.AddMessageQueue(config =>
{
    config.AddAzureServiceBus("Asd", opt => configuration.GetSection("Mq:AzureServiceBus:Asd").Bind(opt));

    config.AddPublisher(pub =>
    {
        pub.AddAzureServiceBusEventPublisher<SomeEvent>(
            "Asd", 
            "my-topic",
            messageInterceptor: (@event, message) =>
            {
                message.CorrelationId = @event.ToString();
            });
        // pub.AddOutboxStore<AppDbContext>();
        // pub.AddOutboxMessageProducer<AppDbContext>();
    });

    config.AddReceiver(rec =>
    {
        rec.AddAzureServiceBusTopicReceiver<SomeEvent>("Asd", "topic", "subscription")
            .AddFuncHandler(HandleAnyMessage);
        rec.AddAzureServiceBusQueueReceiver<SomeEvent>("Asd", "queue")
            .AddFuncHandler(HandleAnyMessage);
    });
});

await using var sp = services.BuildServiceProvider();
var publisher = sp.GetRequiredService<IPublisher>();

await publisher.Publish(new SomeEvent(), CancellationToken.None);


static Task<bool> HandleAnyMessage<T>(IServiceProvider sp, T @event)
{
    sp.GetRequiredService<ILogger>().LogInformation("Received: {@Event}", @event);
    return Task.FromResult(true);
}

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

