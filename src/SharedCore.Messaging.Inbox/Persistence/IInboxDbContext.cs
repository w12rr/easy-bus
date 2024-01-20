using Microsoft.EntityFrameworkCore;
using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox.Persistence;

public interface IInboxDbContext
{
    DbSet<InboxMessage> InboxMessages { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}