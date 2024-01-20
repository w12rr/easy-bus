using Microsoft.EntityFrameworkCore;
using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox.Persistence;

public class InboxRepository : IInboxRepository
{
    private readonly IInboxDbContext _inboxDbContext;

    public InboxRepository(IInboxDbContext inboxDbContext)
    {
        _inboxDbContext = inboxDbContext;
    }

    public void Add(InboxMessage inboxMessage)
    {
        _inboxDbContext.InboxMessages.Add(inboxMessage);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await _inboxDbContext.SaveChangesAsync(cancellationToken);
    }

    public void Remove(InboxMessage next)
    {
        _inboxDbContext.InboxMessages.Remove(next);
    }

    public async Task<IReadOnlyCollection<InboxMessage>> GetIbBoxMessages(CancellationToken cancellationToken)
    {
        var messagesListed = await _inboxDbContext.InboxMessages
            .GroupBy(x => x.CorrelationId)
            .Where(x => !x.Any(c => c.NextReadAt > DateTimeOffset.Now) && !x.Any(c => (c.NextReadAt - c.CreateDate).Minutes > 30))
            .SelectMany(x => x)
            .OrderBy(x => x.Id)
            .Take(30)
            .ToListAsync(cancellationToken);

        return messagesListed.AsReadOnly();
    }
}