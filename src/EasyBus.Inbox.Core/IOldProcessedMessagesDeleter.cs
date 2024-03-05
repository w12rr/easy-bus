namespace EasyBus.Inbox.Core;

public interface IOldProcessedMessagesDeleter
{
    Task DeleteOldProcessed(CancellationToken cancellationToken);
}