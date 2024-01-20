using EasyBus.Core.Definitions;

namespace EasyBus.Core.Receiving;

public interface IMessageReceiver
{
    Task Receive(string message, string correlationId, string messageId, IEventReceiverDefinition definition, CancellationToken cancellationToken);
}