namespace EasyBux.Outbox.Infrastructure;

public sealed class SqlQueriesOptions
{
    public string Delete { get; set; } = default!;
    public string GetNext { get; set; } = default!;
    public string Insert { get; set; } = default!;
    public string CreateTable { get; set; } = default!;
}