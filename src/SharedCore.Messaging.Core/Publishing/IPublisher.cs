namespace SharedCore.Messaging.Core.Publishing;

public interface IPublisher
{
    Task Publish<T>(T @event, CancellationToken cancellationToken);
}