using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Receiving.Processors;
using EasyBus.Core;

namespace EasyBus.AzureServiceBus.Receiving.Definitions;

public class AzureServiceBusTopicEventReceiverDefinition<T> : IAzureServiceBusTopicEventReceiverDefinition
{
    private readonly string _messageQueueName;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private readonly bool _enableSessions;

    public AzureServiceBusTopicEventReceiverDefinition(string messageQueueName, string topicName,
        string subscriptionName, bool enableSessions = false)
    {
        _messageQueueName = messageQueueName;
        _topicName = topicName;
        _subscriptionName = subscriptionName;
        _enableSessions = enableSessions;
    }

    public IMessageQueue SelectMessageQueue(IEnumerable<IMessageQueue> messageQueues) =>
        messageQueues.Single(x => x.Name == _messageQueueName);

    public string GetDefinitionId() => _topicName;

    public IRichServiceBusProcessor GetProcessor(ServiceBusClient client)
    {
        if (_enableSessions)
        {
            return new RichSessionServiceBusProcessor(client.CreateSessionProcessor(_topicName, _subscriptionName));
        }

        return new RichServiceBusProcessor(client.CreateProcessor(_topicName, _subscriptionName));
    }

    public object GetNotification(string message)
    {
        var @event = MqSerializer.Deserialize<T>(message);
        return new object();// new EventWrapper<T>(@event);
    }

    public void LogReceivingError( ProcessErrorEventArgs processErrorEventArgs)
    {
        // logger.LogCritical(
            // processErrorEventArgs.Exception,
            // "Got exception during consuming message {ProcessorId} {QualifiedNameSpace} {EntityPath} {DefinitionTypeName} {EventName}",
            // processErrorEventArgs.Identifier,
            // processErrorEventArgs.FullyQualifiedNamespace,
            // processErrorEventArgs.EntityPath,
            // GetType().Name,
            // typeof(T).Name);
    }
}