namespace EasyBus.Outbox;

public interface IOutboxRepository
{
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxEntity>> GetNext(CancellationToken cancellationToken);
    Task CreateNotExistingTable(CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
}


public class OutboxRepository : IOutboxRepositor y
