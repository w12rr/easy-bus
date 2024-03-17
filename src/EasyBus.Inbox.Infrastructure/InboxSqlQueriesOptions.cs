namespace EasyBus.Inbox.Infrastructure;

public class InboxSqlQueriesOptions
{
    public string Insert { get; set; } = default!;
    public string UpdateTries { get; set; } = default!;
    public string SetReceived { get; set; } = default!;
    public string GetUnprocessed { get; set; } = default!;
    public string DeleteOldProcessed { get; set; } = default!;
    public string CreateTable { get; set; } = default!;
}