namespace EasyBus.Outbox.Core;

public class MissingTableCreator : IMissingTableCreator
{
    private readonly IOutboxRepository _outboxRepository;

    public MissingTableCreator(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }
    
    public async Task Create(CancellationToken cancellationToken)
    {
        await _outboxRepository.CreateNotExistingTable(cancellationToken);
        await _outboxRepository.SaveChanges(cancellationToken);
    }
}