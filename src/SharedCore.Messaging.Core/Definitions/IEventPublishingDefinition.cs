namespace SharedCore.Messaging.Core.Definitions;

public interface IEventPublishingDefinition
{
    IMessageQueue SelectMessageQueue(IEnumerable<IMessageQueue> messageQueues);
}

public interface IEventPublishingDefinition<T> : IEventPublishingDefinition
{
}