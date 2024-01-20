using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.Core.Receiving;

public interface IMessageReceiver
{
    Task Receive(string message, string correlationId, string messageId, IEventReceiverDefinition definition, CancellationToken cancellationToken);
}