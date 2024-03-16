using System.Text.Json;
using EasyBus.Core.Helpers;
using Microsoft.Data.SqlClient;

namespace EasyBus.Outbox.Core;

public class OutboxPublisher : IOutboxPublisher
{
    private readonly IOutboxRepository _outboxRepository;

    public OutboxPublisher(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public async Task Publish<T>(T @event, CancellationToken cancellationToken)
        where T : notnull
    {
        var type = @event.GetType().AssemblyQualifiedName.AssertNull();
        var json = JsonSerializer.Serialize(@event);

        await _outboxRepository.Insert(type, json, cancellationToken);
        await _outboxRepository.SaveChanges(cancellationToken);
    }

    public async Task Publish<T>(SqlTransaction _, T @event, CancellationToken cancellationToken)
        where T : notnull
    {
        var type = @event.GetType().AssemblyQualifiedName.AssertNull();
        var json = JsonSerializer.Serialize(@event);

        await _outboxRepository.Insert(type, json, cancellationToken);
        await _outboxRepository.SaveChanges(cancellationToken);
    }
}