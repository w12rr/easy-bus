using EasyBux.Outbox.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Outbox.Databases.SqlServer;

public static class OutboxSqlServerDependencyInjectionExtensions
{
    public static void UseSqlServer(this OutboxConfig config)
    {
        config.Services.AddOptions<SqlQueriesOptions>().Configure(opt =>
        {
            opt.CreateTable = SqlServerQueries.CreateTable;
            opt.Delete = SqlServerQueries.Delete;
            opt.GetNext = SqlServerQueries.GetNext;
            opt.Insert = SqlServerQueries.Insert;
        });
    }
}