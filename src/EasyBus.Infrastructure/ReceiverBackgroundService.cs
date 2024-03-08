using EasyBus.Core.InfrastructureWrappers;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Infrastructure;

public class ReceiverBackgroundService : BackgroundService
{
    private readonly IReadOnlyCollection<IInfrastructureReceiver> _receivers;

    public ReceiverBackgroundService(IEnumerable<IInfrastructureReceiver> receivers)
    {
        _receivers = receivers.ToList().AsReadOnly();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        foreach (var receiver in _receivers)
        {
            await receiver.StartReceiving(stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        Console.WriteLine("Camcelled");
    }
}