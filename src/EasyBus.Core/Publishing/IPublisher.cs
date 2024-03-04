namespace EasyBus.Core.Publishing;

public interface IPublisher
{
    Task Publish<T>(T @event, CancellationToken cancellationToken);
    Task Publish(object @event, CancellationToken cancellationToken);
}