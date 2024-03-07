namespace EasyBus.Core.InfrastructureWrappers;

public interface IInfrastructureReceiver 
{
    Task StartReceiving(CancellationToken cancellationToken);
}

