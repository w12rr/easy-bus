namespace EasyBus.Inbox.Core;

public interface IInboxMessageReceiver
{
    Task<InboxMessageState> Receive(object @event, CancellationToken cancellationToken);
}

public interface IInboxMessageReceiver<in T> : IInboxMessageReceiver
{
    Task<InboxMessageState> Receive(T @event, CancellationToken cancellationToken);
}