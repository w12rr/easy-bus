namespace EasyBus.Outbox.Core;

public interface IMissingTableCreator
{
    Task Create(CancellationToken cancellationToken);
}