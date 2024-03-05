namespace EasyBus.Outbox;

public sealed record OutboxEntity
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Data { get; init; }
}