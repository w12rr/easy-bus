using MediatR;

namespace SharedCore.Messaging.Core.Definitions;

public interface IEventReceiverDefinition
{
    INotification GetNotification(string message);
    IMessageQueue SelectMessageQueue(IEnumerable<IMessageQueue> messageQueues);
    string GetDefinitionId();
}