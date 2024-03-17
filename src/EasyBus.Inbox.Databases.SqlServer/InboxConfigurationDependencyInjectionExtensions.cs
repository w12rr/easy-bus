using EasyBus.Inbox.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Databases.SqlServer;

public static class InboxConfigurationDependencyInjectionExtensions
{
    public static void UseSqlServer(this InboxConfiguration config)
    {
        config.Services.AddOptions<InboxSqlQueriesOptions>().Configure(opt =>
        {
            opt.CreateTable = SqlServerQueries.CreateTable;
            opt.Insert = SqlServerQueries.Insert;
            opt.GetUnprocessed = SqlServerQueries.GetUnprocessed;
            opt.SetReceived = SqlServerQueries.SetReceived;
            opt.DeleteOldProcessed = SqlServerQueries.DeleteOldProcessed;
            opt.UpdateTries = SqlServerQueries.UpdateTries;
        });
    }
}