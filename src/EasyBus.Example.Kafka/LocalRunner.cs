using EasyBus.Core.Publishing;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Example.Kafka;

public class LocalRunner : BackgroundService
{
    private readonly IPublisher _publisher;

    public LocalRunner(IPublisher publisher)
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