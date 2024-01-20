using SharedCore.Abstraction.Entities;

namespace SharedCore.Messaging.Inbox.Persistence.Entities;

public class InboxMessage : IAuditableEntity, IIdentifiableEntity<long>
{
    public long Id { get; set; }
    public string? CorrelationId { get; set; }
    public string Message { get; set; } = default!;
    public string DefinitionId { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public DateTimeOffset NextReadAt { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public DateTimeOffset? ModifyDate { get; set; }
}