namespace SharedCore.Messaging.Inbox.Consumer;

public interface IInboxMessageConsumer
{
    Task Consume(CancellationToken cancellationToken);
}