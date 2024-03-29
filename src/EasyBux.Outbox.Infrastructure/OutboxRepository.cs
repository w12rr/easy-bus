﻿using System.Data.Common;
using System.Transactions;
using EasyBus.Core.Helpers;
using EasyBus.Outbox.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EasyBux.Outbox.Infrastructure;

public class OutboxRepository : IOutboxRepository, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;
    private readonly SqlQueriesOptions _sqlQueriesOptions;

    public OutboxRepository(IOptions<OutboxConnectionStringOptions> connectionStringOptions,
        IOptions<SqlQueriesOptions> options)
    {
        _connectionString = connectionStringOptions.Value.ConnectionString;
        _sqlQueriesOptions = options.Value;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(_sqlQueriesOptions.Delete, transaction.Connection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OutboxEntity>> GetNext(CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(_sqlQueriesOptions.GetNext, transaction.Connection, transaction);
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
        await using var cmd = new SqlCommand(_sqlQueriesOptions.CreateTable, transaction.Connection, transaction);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task Insert(string type, string data, CancellationToken cancellationToken)
    {
        var transaction = await StartConnectionIfNotStarted(cancellationToken);
        await using var cmd = new SqlCommand(_sqlQueriesOptions.Insert, transaction.Connection, transaction);
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

    public async Task DiscardChanges(CancellationToken cancellationToken)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(cancellationToken);

        if (_connection != null) await _connection.DisposeAsync();
        if (_transaction != null) await _transaction.DisposeAsync();

        _transaction = default;
        _connection = default; //todo ok w ten sposob?
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