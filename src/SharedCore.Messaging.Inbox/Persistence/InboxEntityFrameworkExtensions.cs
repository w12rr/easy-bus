using Microsoft.EntityFrameworkCore;

namespace SharedCore.Messaging.Inbox.Persistence;

public static class InboxEntityFrameworkExtensions
{
    public static void ConfigureInboxEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InboxMessageEntityTypeConfiguration());
    }
}