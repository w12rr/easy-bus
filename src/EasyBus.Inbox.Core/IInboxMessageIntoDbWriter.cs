namespace EasyBus.Inbox.Core;

public interface IInboxMessageIntoDbWriter<in T>
{
    Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken);
}