namespace EasyBus.Core.InfrastructureWrappers;

public interface IInfrastructureReceiver : IAsyncDisposable
{
    Task StartReceiving(CancellationToken cancellationToken);
}

