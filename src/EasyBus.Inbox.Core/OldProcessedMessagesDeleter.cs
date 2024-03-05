namespace EasyBus.Inbox.Core;

public class OldProcessedMessagesDeleter : IOldProcessedMessagesDeleter
{
    private readonly IInboxRepository _inboxRepository;

    public OldProcessedMessagesDeleter(IInboxRepository inboxRepository)
    {
        _inboxRepository = inboxRepository;
    }

    public async Task DeleteOldProcessed(CancellationToken cancellationToken)
    {
        const int messagesOlderThanDays = 50;
        var olderThan = TimeSpan.FromDays(messagesOlderThanDays);
        await _inboxRepository.DeleteOldReceivedMessages(olderThan, cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
    }
}