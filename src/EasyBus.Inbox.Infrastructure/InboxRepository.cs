using EasyBus.Core.Helpers;
using EasyBus.Inbox.Core;
using Microsoft.Data.SqlClient;

namespace EasyBus.Inbox.Infrastructure;

public class InboxRepository : IInboxRepository, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public InboxRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateTable(CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.CreateTable, transaction.Connection, transaction);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteOldReceivedMessages(TimeSpan olderThan, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.DeleteOldProcessed, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Date", DateTime.Now.Add(olderThan.Negate()));
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InboxEntity>> GetUnprocessedMessages(CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
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
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.SetReceived, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateTries(int nextTriesCount, DateTime nextPickupDate, Guid id,
        CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.UpdateTries, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@TriesCount", nextTriesCount);
        cmd.Parameters.AddWithValue("@PickupDate", nextPickupDate);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task Insert(string messageId, string type, string data, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.Insert, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@MessageId", messageId);
        cmd.Parameters.AddWithValue("@Type", type);
        cmd.Parameters.AddWithValue("@Data", data);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await _transaction.AssertNull().CommitAsync(cancellationToken);
        
        if (_connection != null) await _connection.DisposeAsync();
        if (_transaction != null) await _transaction.DisposeAsync();
        
        _transaction = default;
        _connection = default; //todo ok w ten sposob?
    }

    //todo discard changes
    
    private async Task<SqlTransaction> StartConnectionIfNotStarted(CancellationToken cancellationToken)
    {
        if (_transaction is not null) return _transaction;

        _connection = new SqlConnection(_connectionString);
        await _connection.OpenAsync(cancellationToken);
        _transaction = _connection.BeginTransaction();

        return _transaction;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_transaction != null) await _transaction.DisposeAsync();
    }
}