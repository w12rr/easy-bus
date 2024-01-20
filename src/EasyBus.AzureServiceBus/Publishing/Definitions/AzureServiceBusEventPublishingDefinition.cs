using Azure.Messaging.ServiceBus;
using EasyBus.Core;

namespace EasyBus.AzureServiceBus.Publishing.Definitions;

public class AzureServiceBusEventPublishingDefinition<T> : IAzureServiceBusEventPublishingDefinition<T>
{
    private readonly string _messageQueueName;
    private readonly string _topicOrQueueName;
    private readonly Func<T, string>? _correlationIdFactory;

    public AzureServiceBusEventPublishingDefinition(string messageQueueName, string topicOrQueueName,
        Func<T, string>? correlationIdFactory)
    {
        _messageQueueName = messageQueueName;
        _topicOrQueueName = topicOrQueueName;
        _correlationIdFactory = correlationIdFactory;
    }

    public IMessageQueue SelectMessageQueue(IEnumerable<IMessageQueue> messageQueues) =>
        messageQueues.Single(x => x.Name == _messageQueueName);

    public ServiceBusSender CreateSender(ServiceBusClient client) => client.CreateSender(_topicOrQueueName);

    public ServiceBusMessage CreateMessage(T @event)
    {
        return new ServiceBusMessage(MqSerializer.Serialize(@event))
        {
            CorrelationId = _correlationIdFactory?.Invoke(@event),
            MessageId = _correlationIdFactory?.Invoke(@event)
        };
    }
}