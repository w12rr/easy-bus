using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyBus.Inbox.Infrastructure;

public class InboxOldMessagesDeleterBackgroundService : BackgroundService
{
    private readonly string _dbConnectionString;
    private readonly ILogger<InboxOldMessagesDeleterBackgroundService> _logger;

    public InboxOldMessagesDeleterBackgroundService(string dbConnectionString,
        ILogger<InboxOldMessagesDeleterBackgroundService> logger)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var messagesOlderThanDays = 50;
            await using var connection = new SqlConnection(_dbConnectionString);
            SqlTransaction? transaction = null;
            try
            {
                await connection.OpenAsync(cancellationToken);
                transaction = connection.BeginTransaction();
                var dateToCompare = DateTime.Now.AddDays(-messagesOlderThanDays);
                await using var cmd = new SqlCommand(
                    "DELETE FROM [dbo].[Inboxes] WHERE Received = 1 AND InsertDate < @Date",
                    connection,
                    transaction);
                cmd.Parameters.AddWithValue("@Date", dateToCompare);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gor error during creating outbox table");
                try
                {
                    transaction?.Rollback();
                }
                catch (Exception ex2)
                {
                    _logger.LogError(ex2, "Gor error during creating outbox table and rollbacking");
                }
            }

            await Task.Delay(5000, cancellationToken);
        }
    }
}