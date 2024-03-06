using EasyBus.Outbox.Core;
using Microsoft.Extensions.Hosting;

namespace EasyBux.Outbox.Infrastructure;

//todo refactor for this module
public class OutboxBackgroundService : BackgroundService
{
    private readonly IMissingTableCreator _missingTableCreator;
    private readonly IOutboxFromDbToTransportWriter _outboxFromDbToTransportWriter;

    public OutboxBackgroundService(IMissingTableCreator missingTableCreator,
        IOutboxFromDbToTransportWriter outboxFromDbToTransportWriter)
    {
        _missingTableCreator = missingTableCreator;
        _outboxFromDbToTransportWriter = outboxFromDbToTransportWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await _missingTableCreator.Create(stoppingToken);
        await RunPublishing(stoppingToken);
    }

    private async Task RunPublishing(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _outboxFromDbToTransportWriter.Run(cancellationToken);
            await Task.Delay(500, cancellationToken);
        }
    }
}