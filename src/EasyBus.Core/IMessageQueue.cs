using EasyBus.Core.Definitions;

namespace EasyBus.Core;

public interface IMessageQueue 
{ 
    public string Name { get; }
    Task Publish<T>(IEventPublishingDefinition<T> definition, T @event, CancellationToken cancellationToken);
    Task StartReceiving(IEventReceiverDefinition definition, CancellationToken cancellationToken);
    Task StopReceiving(CancellationToken cancellationToken);
}