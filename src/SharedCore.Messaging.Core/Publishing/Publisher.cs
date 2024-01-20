using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.Core.Publishing;

public sealed class Publisher : IPublisher
{
    private readonly IReadOnlyCollection<IEventPublishingDefinition> _eventPublishingDefinitions;
    private readonly IReadOnlyCollection<IMessageQueue> _messageQueues;

    public Publisher(
        IReadOnlyCollection<IEventPublishingDefinition> eventPublishingDefinitions,
        IReadOnlyCollection<IMessageQueue> messageQueues)
    {
        _eventPublishingDefinitions = eventPublishingDefinitions;
        _messageQueues = messageQueues;
    }
    
    public async Task Publish<T>(T @event, CancellationToken cancellationToken)
    {
        var definition = _eventPublishingDefinitions.OfType<IEventPublishingDefinition<T>>().Single();
        var mq = definition.SelectMessageQueue(_messageQueues);
        await mq.Publish(definition, @event, cancellationToken);
    }
}