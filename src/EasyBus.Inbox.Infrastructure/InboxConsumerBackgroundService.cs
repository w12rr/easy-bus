using EasyBus.Inbox.Core;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Inbox.Infrastructure;

public class InboxConsumerBackgroundService : BackgroundService
{
    private readonly IConsumingRunner _consumingRunner;

    public InboxConsumerBackgroundService(IConsumingRunner consumingRunner)
    {
        _consumingRunner = consumingRunner;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await RunProcessing(stoppingToken);
    }

    private async Task RunProcessing(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested) //todo try/catch
        {
            await _consumingRunner.Run(cancellationToken); //todo keep order of correlation id
            await Task.Delay(500, cancellationToken);
        }
    }
}