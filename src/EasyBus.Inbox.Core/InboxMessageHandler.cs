namespace EasyBus.Inbox.Core;

public class InboxMessageHandler<T> : IInboxMessageHandler<T>
{
    public Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}