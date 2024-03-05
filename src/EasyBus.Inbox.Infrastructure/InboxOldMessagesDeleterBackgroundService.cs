using EasyBus.Inbox.Core;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Inbox.Infrastructure;

public class InboxOldMessagesDeleterBackgroundService : BackgroundService
{
    private readonly IOldProcessedMessagesDeleter _processedMessagesDeleter;

    public InboxOldMessagesDeleterBackgroundService(IOldProcessedMessagesDeleter processedMessagesDeleter)
    {
        _processedMessagesDeleter = processedMessagesDeleter;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _processedMessagesDeleter.DeleteOldProcessed(cancellationToken);
            await Task.Delay(5000, cancellationToken);
        }
    }
}