using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Options;
using EasyBus.Core.InfrastructureWrappers;

namespace EasyBus.AzureServiceBus.Publishing;

public class AzureServiceBusInfrastructurePublisher<T> : IInfrastructurePublisher<T>
{
    private readonly ServiceBusConnectionOptions _serviceBusConnectionOptions;
    private readonly string _topicOrQueueName;

    public AzureServiceBusInfrastructurePublisher(
        ServiceBusConnectionOptions serviceBusConnectionOptions,
        string topicOrQueueName)
    {
        _serviceBusConnectionOptions = serviceBusConnectionOptions;
        _topicOrQueueName = topicOrQueueName;
    }
    
    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var client = new ServiceBusClient(
            _serviceBusConnectionOptions.ConnectionString,
            new DefaultAzureCredential(),
            new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });
        var sender = client.CreateSender(_topicOrQueueName);
        await sender.SendMessageAsync(
            new ServiceBusMessage(JsonSerializer.Serialize(@event)),
            cancellationToken);
    }
}

