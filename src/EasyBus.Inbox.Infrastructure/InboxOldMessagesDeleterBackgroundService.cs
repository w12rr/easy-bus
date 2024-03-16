using EasyBus.Inbox.Core;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Inbox.Infrastructure;

public class InboxOldMessagesDeleterBackgroundService : BackgroundService
{
    private readonly IOldProcessedMessagesDeleter _processedMessagesDeleter;
    private readonly IMissingOutboxTableCreator _missingOutboxTableCreator;

    public InboxOldMessagesDeleterBackgroundService(IOldProcessedMessagesDeleter processedMessagesDeleter,
        IMissingOutboxTableCreator missingOutboxTableCreator)
    {
        _processedMessagesDeleter = processedMessagesDeleter;
        _missingOutboxTableCreator = missingOutboxTableCreator;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _missingOutboxTableCreator.Create(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                await _processedMessagesDeleter.DeleteOldProcessed(cancellationToken);
                await Task.Delay(5000, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}