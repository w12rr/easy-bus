using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EasyBus.Outbox;

public class OutboxPublisher
    : IOutboxPublisher
{
    private readonly string _dbConnectionString;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(string dbConnectionString,
        ILogger<OutboxPublisher> logger)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger;
    }

    public async Task Publish<T>(T @event, CancellationToken cancellationToken)
        where T : notnull
    {
        await using var connection = new SqlConnection(_dbConnectionString);
        SqlTransaction? transaction = null;
        try
        {
            await connection.OpenAsync(cancellationToken);
            transaction = connection.BeginTransaction();
            await using var cmd = new SqlCommand(
                "INSERT INTO [dbo].[Outboxes] (Type, Data) VALUES (@Type, @Data)",
                connection,
                transaction);
            cmd.Parameters.AddWithValue("@Type", @event.GetType().AssemblyQualifiedName);
            cmd.Parameters.AddWithValue("@Data", JsonSerializer.Serialize(@event));

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gor error during publishing to outbox {EventName}", typeof(T).Name);
            try
            {
                transaction?.Rollback();
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Gor error during publishing to outbox and rollbacking {EventName}",
                    typeof(T).Name);
            }
        }
    }

    public async Task Publish<T>(SqlTransaction transaction, T @event, CancellationToken cancellationToken)
        where T : notnull
    {
        try
        {
            await using var cmd = new SqlCommand(
                "INSERT INTO [dbo].[Outboxes] (Type, Data) VALUES (@Type, @Data)",
                transaction.Connection,
                transaction);
            cmd.Parameters.AddWithValue("@Type", @event.GetType().AssemblyQualifiedName);
            cmd.Parameters.AddWithValue("@Data", JsonSerializer.Serialize(@event));

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gor error during publishing to outbox {EventName}", typeof(T).Name);
            try
            {
                transaction.Rollback();
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Gor error during publishing to outbox and rollbacking {EventName}",
                    typeof(T).Name);
            }
        }
    }
}