using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox.Persistence;

public interface IInboxRepository
{
    void Add(InboxMessage inboxMessage);
    Task<IReadOnlyCollection<InboxMessage>> GetIbBoxMessages(CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
    void Remove(InboxMessage next);
}