using Microsoft.Extensions.Hosting;
using SharedCore.Messaging.Inbox.Consumer;

namespace SharedCore.Messaging.Inbox.HostedService;

public class InboxHostedService : BackgroundService
{
    private readonly IInboxMessageConsumer _messageConsumer;

    public InboxHostedService(IInboxMessageConsumer messageConsumer)
    {
        _messageConsumer = messageConsumer;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await _messageConsumer.Consume(stoppingToken);
    }
}