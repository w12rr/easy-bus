using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.Core.Receiving;

public class Receiver : IReceiver
{
    private readonly IReadOnlyCollection<IEventReceiverDefinition> _eventReceiverDefinitions;
    private readonly IReadOnlyCollection<IMessageQueue> _messageQueues;

    public Receiver(IReadOnlyCollection<IEventReceiverDefinition> eventReceiverDefinitions,
        IReadOnlyCollection<IMessageQueue> messageQueues)
    {
        _eventReceiverDefinitions = eventReceiverDefinitions;
        _messageQueues = messageQueues;
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {//todo listen for dlq
        foreach (var definition in _eventReceiverDefinitions)
        {
            var mq = definition.SelectMessageQueue(_messageQueues);
            await mq.StartReceiving(definition, cancellationToken);
        }
    }

    public async Task StopReceiving(CancellationToken cancellationToken)
    {
        foreach (var mq in _messageQueues)
        {
            await mq.StopReceiving(cancellationToken);
        }
    }
}