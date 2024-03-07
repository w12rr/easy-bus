namespace EasyBus.Inbox.Core;

public class MissingOutboxTableCreator : IMissingOutboxTableCreator
{
    private readonly IInboxRepository _inboxRepository;

    public MissingOutboxTableCreator(IInboxRepository inboxRepository)
    {
        _inboxRepository = inboxRepository;
    }
    
    public async Task Create(CancellationToken cancellationToken)
    {
        await _inboxRepository.CreateTable(cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
    }
}