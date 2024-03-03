using System.Text.Json;
using EasyBus.Core.Helpers;
using EasyBus.Core.Publishing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyBus.Outbox;

public interface IOutboxPublisher
{
    Task Publish<T>(T @event, CancellationToken cancellationToken)
        where T : notnull;

    Task Publish<T>(SqlTransaction transaction, T @event, CancellationToken cancellationToken)
        where T : notnull;
}

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

public class OutboxHostedService : BackgroundService
{
    private readonly string _connectionString;
    private readonly ILogger<OutboxHostedService> _logger;
    private readonly IPublisher _publisher;

    public OutboxHostedService(string connectionString, ILogger<OutboxHostedService> logger, IPublisher publisher)
    {
        _connectionString = connectionString;
        _logger = logger;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await CreateIfNotExists(stoppingToken);
        await RunPublishing(stoppingToken);
    }

    private async Task RunPublishing(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        SqlTransaction? transaction = null;
        try
        {
            await connection.OpenAsync(cancellationToken);
            transaction = connection.BeginTransaction();
            await RunPublishingWithData(connection, transaction, cancellationToken);
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
    }

    private async Task RunPublishingWithData(SqlConnection sqlConnection, SqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        var next = await GetNext(sqlConnection, transaction, cancellationToken);

        foreach (var data in next)
        {
            var type = Type.GetType(data.type).AssertNull();
            var deserialized = JsonSerializer.Deserialize(data.data, type).AssertNull();
            await _publisher.P ublish(deserialized, cancellationToken); //todo cannot send object type, todo cannot find object based on object type reflection?
        }
    }

    private static async Task<(Guid id, string type, string data, int tryCount)[]> GetNext(SqlConnection sqlConnection,
        SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand("SELECT Id, Type, Data, PublishTryCount  FROM [dbo].Outboxes",
            sqlConnection, transaction);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var data = new List<(Guid id, string type, string data, int tryCount)>();

        while (await reader.ReadAsync(cancellationToken))
        {
            data.Add((
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3)
            ));
        }

        return data.ToArray();
    }

    private async Task CreateIfNotExists(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        SqlTransaction? transaction = null;
        try
        {
            await connection.OpenAsync(cancellationToken);
            transaction = connection.BeginTransaction();
            await using var cmd = new SqlCommand(
                """
                    IF (NOT EXISTS (SELECT *
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_SCHEMA = 'dbo'
                            AND  TABLE_NAME = 'Outboxes'))
                    BEGIN
                        CREATE TABLE [dbo].[Outboxes](
                	        [Id] [uniqueidentifier] NOT NULL,
                	        [Type] [nvarchar](max) NOT NULL,
                	        [Data] [nvarchar](max) NOT NULL,
                	        [InsertDate] [datetime] NOT NULL DEFAULT GETDATE(),
                	        [PublishTryCount] [int] NOT NULL DEFAULT 0,
                            CONSTRAINT [PK_Outboxes] PRIMARY KEY CLUSTERED
                            (
                	            [Id] ASC
                            ) WITH (
                                    PAD_INDEX = OFF,
                                    STATISTICS_NORECOMPUTE = OFF,
                                    IGNORE_DUP_KEY = OFF,
                                    ALLOW_ROW_LOCKS = ON,
                                    ALLOW_PAGE_LOCKS = ON,
                                    OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
                                ) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    END
                """,
                connection,
                transaction);
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
    }
}