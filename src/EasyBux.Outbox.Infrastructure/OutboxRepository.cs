using EasyBus.Core.Helpers;
using EasyBus.Outbox.Core;
using Microsoft.Data.SqlClient;

namespace EasyBux.Outbox.Infrastructure;

public class OutboxRepository : IOutboxRepository, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public OutboxRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.Delete, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OutboxEntity>> GetNext(CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.GetNext, transaction.Connection, transaction);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var data = new List<OutboxEntity>();
        while (await reader.ReadAsync(cancellationToken))
        {
            data.Add(new OutboxEntity
            {
                Id = reader.GetGuid(0),
                Type = reader.GetString(1),
                Data = reader.GetString(2)
            });
        }
        return data;
    }

    public async Task CreateNotExistingTable(CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.CreateTable, transaction.Connection, transaction);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task Insert(string type, string data, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(SqlServerQueries.Insert, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Type", type);
        cmd.Parameters.AddWithValue("@Type", data);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await _transaction.AssertNull().CommitAsync(cancellationToken);
    }

    public async Task DiscardChanges(CancellationToken cancellationToken)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(cancellationToken);
    }

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