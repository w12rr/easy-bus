using System.Text.Json;
using EasyBus.Core.Helpers;
using EasyBus.Inbox.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyBus.Inbox.Infrastructure;

public class InboxConsumerBackgroundService : BackgroundService
{
    private readonly string _dbConnectionString;
    private readonly ILogger<InboxConsumerBackgroundService> _logger;
    private readonly IReadOnlyCollection<IInboxMessageReceiver> _messageReceivers;

    public InboxConsumerBackgroundService(string dbConnectionString, ILogger<InboxConsumerBackgroundService> logger,
        IReadOnlyCollection<IInboxMessageReceiver> messageReceivers)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger;
        _messageReceivers = messageReceivers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await RunProcessing(stoppingToken);
    }

    private async Task RunProcessing(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var connection = new SqlConnection(_dbConnectionString);
            SqlTransaction? transaction = null;
            try
            {
                await connection.OpenAsync(cancellationToken);
                transaction = connection.BeginTransaction();

                var next = await GetNext(connection, transaction,
                    cancellationToken); //todo keep order of correlation id
                await ProcessNext(next, connection, transaction, cancellationToken);
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

    private async Task ProcessNext(IEnumerable<InboxEntity> next, SqlConnection connection,
        SqlTransaction transaction, CancellationToken cancellationToken)
    {
        foreach (var message in next)
        {
            var targetEventType = Type.GetType(message.Type).AssertNull();
            var targetReceiverType = typeof(IInboxMessageReceiver<>).MakeGenericType(targetEventType);
            var receiver = _messageReceivers.Single(x => x.GetType().IsAssignableTo(targetReceiverType));

            var targetEventObject = JsonSerializer.Deserialize(message.Data, targetEventType).AssertNull();
            var inboxState = await receiver.Receive(targetEventObject, cancellationToken);

            if (inboxState is InboxMessageState.NotReceived)
            {
                await UpdatePickupDate(message, connection, transaction, cancellationToken);
            }

            if (inboxState is InboxMessageState.Received)
            {
                await SetReceived(message.Id, connection, transaction, cancellationToken);
            }

            throw new Exception("Invalid inbox action");
        }
    }

    public static async Task UpdatePickupDate(InboxEntity entity, SqlConnection sqlConnection,
        SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand(
            "UPDATE [dbo].[Inboxes] SET TriesCount = @TriesCount, PickupDate = @PickupDate WHERE Id = @Id",
            sqlConnection,
            transaction);
        cmd.Parameters.AddWithValue("@TriesCount", entity.TriesCount + 1);
        cmd.Parameters.AddWithValue("@Id", entity.Id);
        cmd.Parameters.AddWithValue("@PickupDate", entity.InsertDate.AddSeconds(Math.Pow(2, entity.TriesCount)));

        await cmd.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public static async Task SetReceived(Guid id, SqlConnection sqlConnection, SqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand(
            "UPDATE [dbo].Outboxes SET Received = 1 WHERE Id = @Id",
            sqlConnection,
            transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task<IEnumerable<InboxEntity>> GetNext(SqlConnection sqlConnection,
        SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await using var cmd = new SqlCommand(
            "SELECT Id, Type, Data, TriesCount FROM [dbo].[Inboxes] WHERE Received = 0 AND PickupDate <= GETDATE()",
            sqlConnection, transaction);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var data = new List<InboxEntity>();

        while (await reader.ReadAsync(cancellationToken))
        {
            data.Add(
                new InboxEntity(
                    reader.GetGuid(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.GetDateTime(4)));
        }

        return data.ToArray();
    }
}