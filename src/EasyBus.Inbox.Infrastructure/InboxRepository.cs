using EasyBus.Core.Helpers;
using EasyBus.Inbox.Core;
using Microsoft.Data.SqlClient;

namespace EasyBus.Inbox.Infrastructure;

public static class SqlServerQueries
{
    public const string Insert =
        "INSERT INTO [dbo].[Inboxes] (MessageId, Type, Data) VALUES (@MessageId, @Type, @Data)";

    public const string UpdateTries =
        "UPDATE [dbo].[Inboxes] SET TriesCount = @TriesCount, PickupDate = @PickupDate WHERE Id = @Id";

    public const string SetReceived = "UPDATE [dbo].Outboxes SET Received = 1 WHERE Id = @Id";

    public const string GetUnprocessed =
        "SELECT Id, Type, Data, TriesCount FROM [dbo].[Inboxes] WHERE Received = 0 AND PickupDate <= GETDATE() AND TriesCount < 11";

    public const string DeleteOldProcessed = "DELETE FROM [dbo].[Inboxes] WHERE Received = 1 AND InsertDate < @Date";
}

public class InboxRepository : IInboxRepository, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public InboxRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task CreateTable(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteOldReceivedMessages(TimeSpan olderThan, CancellationToken cancellationToken)
    {
        var transaction = StartConnectionIfNotStarted();
        await using var cmd = new SqlCommand(SqlServerQueries.DeleteOldProcessed, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Date", DateTime.Now.Add(olderThan.Negate()));
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InboxEntity>> GetUnprocessedMessages(CancellationToken cancellationToken)
    {
        var transaction = StartConnectionIfNotStarted();
        await using var cmd = new SqlCommand(SqlServerQueries.GetUnprocessed, transaction.Connection, transaction);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var result = new List<InboxEntity>();

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new InboxEntity(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetDateTime(4)));
        }

        return result;
    }

    public async Task SetProcessed(Guid id, CancellationToken cancellationToken)
    {
        var transaction = StartConnectionIfNotStarted();
        await using var cmd = new SqlCommand(SqlServerQueries.SetReceived, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateTries(int nextTriesCount, DateTime nextPickupDate, Guid id,
        CancellationToken cancellationToken)
    {
        var transaction = StartConnectionIfNotStarted();
        await using var cmd = new SqlCommand(SqlServerQueries.UpdateTries, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@TriesCount", nextTriesCount);
        cmd.Parameters.AddWithValue("@PickupDate", nextPickupDate);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task Insert(string messageId, string type, string data, CancellationToken cancellationToken)
    {
        var transaction = StartConnectionIfNotStarted();
        await using var cmd = new SqlCommand(SqlServerQueries.Insert, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@MessageId", messageId);
        cmd.Parameters.AddWithValue("@Type", type);
        cmd.Parameters.AddWithValue("@Data", data);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await _transaction.AssertNull().CommitAsync(cancellationToken);
    }

    private SqlTransaction StartConnectionIfNotStarted()
    {
        if (_transaction is not null) return _transaction;

        _connection = new SqlConnection(_connectionString);
        _transaction = _connection.BeginTransaction();

        return _transaction;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_transaction != null) await _transaction.DisposeAsync();
    }
}