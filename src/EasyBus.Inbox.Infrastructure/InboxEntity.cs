namespace EasyBus.Inbox.Infrastructure;

public sealed record InboxEntity(Guid Id, string Type, string Data, int TriesCount, DateTime InsertDate);