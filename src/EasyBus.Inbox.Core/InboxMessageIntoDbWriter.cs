using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EasyBus.Inbox.Core;

public class InboxMessageIntoDbWriter<T> : IInboxMessageIntoDbWriter<T>
{
    private readonly string _dbConnectionString;
    private readonly ILogger<InboxMessageIntoDbWriter<T>> _logger;
    private readonly Func<T, string> _messageIdProvider;

    public InboxMessageIntoDbWriter(string dbConnectionString, 
        ILogger<InboxMessageIntoDbWriter<T>> logger,
        Func<T, string> messageIdProvider)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger;
        _messageIdProvider = messageIdProvider;
    }
    
    public async Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_dbConnectionString);
        SqlTransaction? transaction = null;
        try
        {
            await connection.OpenAsync(cancellationToken);
            transaction = connection.BeginTransaction();
            await SaveToDb(connection, transaction, @event, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        //catch () //todo catch duplicated key
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

        return false;
    }

    private async Task SaveToDb(
        SqlConnection connection, 
        SqlTransaction transaction, 
        T @event,
        CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand(
            """
                INSERT INTO [dbo].[Inboxes] (MessageId, Type, Data)
                VALUES (@MessageId, @Type, @Data)
            """, 
            connection, 
            transaction);
        cmd.Parameters.AddWithValue("@MessageId", _messageIdProvider(@event));
        cmd.Parameters.AddWithValue("@Type", typeof(T).AssemblyQualifiedName);
        cmd.Parameters.AddWithValue("@Data", JsonSerializer.Serialize(@event));
        
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}