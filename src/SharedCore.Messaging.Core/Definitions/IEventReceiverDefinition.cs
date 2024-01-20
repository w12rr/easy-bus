namespace SharedCore.Messaging.Core.Definitions;

public interface IEventReceiverDefinition
{
    object GetNotification(string message);
    IMessageQueue SelectMessageQueue(IEnumerable<IMessageQueue> messageQueues);
    string GetDefinitionId();
}

