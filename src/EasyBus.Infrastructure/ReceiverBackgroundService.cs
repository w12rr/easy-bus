using EasyBus.Core.InfrastructureWrappers;
using Microsoft.Extensions.Hosting;

namespace EasyBus.Infrastructure;

public class ReceiverBackgroundService : BackgroundService
{
    private readonly IReadOnlyCollection<IInfrastructureReceiver> _receivers;

    public ReceiverBackgroundService(IReadOnlyCollection<IInfrastructureReceiver> receivers)
    {
        _receivers = receivers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var receiver in _receivers)
        {
            await receiver.StartReceiving(stoppingToken);
        }
    }
}