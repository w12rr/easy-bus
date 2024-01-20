using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox.Persistence;

public class InboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.HasIndex(x => new { x.MessageId, x.DefinitionId }).IsUnique();
    }
}