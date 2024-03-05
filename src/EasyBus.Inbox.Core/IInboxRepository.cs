namespace EasyBus.Inbox.Core;

public interface IInboxRepository
{
    Task CreateTable(CancellationToken cancellationToken);
    Task DeleteOldReceivedMessages(TimeSpan olderThan, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<InboxEntity>> GetUnprocessedMessages(CancellationToken cancellationToken);
    Task SetProcessed(Guid id, CancellationToken cancellationToken);
    Task UpdateTries(int nextTriesCount, DateTime nextPickupDate, Guid id, CancellationToken cancellationToken);
    Task Insert(string messageId, string type, string data, CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
}