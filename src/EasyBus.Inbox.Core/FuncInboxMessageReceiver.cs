namespace EasyBus.Inbox.Core;

public class FuncInboxMessageReceiver<T> : IInboxMessageReceiver<T>
{
    private readonly Func<T, CancellationToken, Task<InboxMessageAction>> _handler;

    public FuncInboxMessageReceiver(Func<T, CancellationToken, Task<InboxMessageAction>> handler)
    {
        _handler = handler;
    }

    public async Task<InboxMessageAction> Receive(T @event, CancellationToken cancellationToken)
    {
        return await _handler(@event, cancellationToken);
    }
}