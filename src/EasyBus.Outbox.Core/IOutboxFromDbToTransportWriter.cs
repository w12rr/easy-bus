namespace EasyBus.Outbox.Core;

public interface IOutboxFromDbToTransportWriter
{
    Task Run(CancellationToken cancellationToken);
}