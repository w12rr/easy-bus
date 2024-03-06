namespace EasyBus.Outbox.Core;

public interface IOutboxRepository
{
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxEntity>> GetNext(CancellationToken cancellationToken);
    Task CreateNotExistingTable(CancellationToken cancellationToken);
    Task Insert(string type, string data, CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
    Task DiscardChanges(CancellationToken cancellationToken);
}