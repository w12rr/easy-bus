namespace EasyBus.Inbox.Core;

public interface IInboxMessageHandler<in T>
{
    Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken);
}