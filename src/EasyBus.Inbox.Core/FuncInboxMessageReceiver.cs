namespace EasyBus.Inbox.Core;

public class FuncInboxMessageReceiver<T> : IInboxMessageReceiver<T> //todo some base class?
{
    private readonly Func<T, CancellationToken, Task<InboxMessageState>> _handler;

    public FuncInboxMessageReceiver(Func<T, CancellationToken, Task<InboxMessageState>> handler)
    {
        _handler = handler;
    }

    public async Task<InboxMessageState> Receive(T @event, CancellationToken cancellationToken)
    {
        return await _handler(@event, cancellationToken);
    }

    public async Task<InboxMessageState> Receive(object @event, CancellationToken cancellationToken)
    {
        return await Receive((T)@event, cancellationToken);
    }
}