using EasyBus.Core.Publishing;
using EasyBus.Outbox.Core;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Example.Kafka;

public class LocalRunner : BackgroundService
{
    private readonly IOutboxPublisher _publisher;

    public LocalRunner(IOutboxPublisher publisher)
    {
        _publisher = publisher;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var inc = 1;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var testEvent = new TestEvent(Guid.NewGuid(), $"this is another message - {inc++}");
            await _publisher.Publish(testEvent, CancellationToken.None);
            await Task.Delay(2000, stoppingToken);
        }
    }
}