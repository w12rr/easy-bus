namespace EasyBus.Inbox.Core;

public interface IInboxMessageReceiver<in T>
{
    Task<InboxMessageAction> Receive(T @event, CancellationToken cancellationToken);
}