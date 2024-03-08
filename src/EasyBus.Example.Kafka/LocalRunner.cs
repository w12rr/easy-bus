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

        await _publisher.Publish(new TestEvent(Guid.NewGuid(), "this is another message"), CancellationToken.None);

        Console.WriteLine("Published");
    }
}