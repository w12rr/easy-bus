using System.Text.Json;
using EasyBus.Core.Helpers;
using EasyBus.Core.Publishing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyBus.Outbox;
//todo refactor for this module
public class OutboxBackgroundService : BackgroundService
{
    private readonly string _connectionString;
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly IPublisher _publisher;

    public OutboxBackgroundService(string connectionString, ILogger<OutboxBackgroundService> logger, IPublisher publisher)
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
        while (!cancellationToken.IsCancellationRequested)
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

            await Task.Delay(500, cancellationToken);
        }
    }

    private async Task RunPublishingWithData(SqlConnection connection, SqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        var next = await GetNext(connection, transaction, cancellationToken);

        //TODO db connection per each interation
        foreach (var data in next)
        {
            var type = Type.GetType(data.type).AssertNull();
            var deserialized = JsonSerializer.Deserialize(data.data, type).AssertNull();
            await DeleteOutbox(data.id, connection, transaction, cancellationToken);
            try
            {
                await _publisher.Publish(deserialized, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "During outbox publishing got an error");
                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex2)
                {
                    _logger.LogCritical(ex2, "During rollback outbox publishing got an error");
                    break;
                }
            }
        }
    }

    private static async Task DeleteOutbox(Guid id, SqlConnection sqlConnection, SqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand("DELETE FROM [dbo].Outboxes WHERE Id = @Id", sqlConnection, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
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