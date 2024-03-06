using System.Text.Json;
using EasyBus.Core.Helpers;
using EasyBus.Core.Publishing;
using Microsoft.Extensions.Logging;

namespace EasyBus.Outbox.Core;

public class OutboxFromDbToTransportWriter : IOutboxFromDbToTransportWriter
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxFromDbToTransportWriter> _logger;

    public OutboxFromDbToTransportWriter(IOutboxRepository outboxRepository, IPublisher publisher,
        ILogger<OutboxFromDbToTransportWriter> logger)
    {
        _outboxRepository = outboxRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var next = await GetNext(cancellationToken);

        //TODO db connection per each interation
        foreach (var data in next)
        {
            var type = Type.GetType(data.Type).AssertNull();
            var deserialized = JsonSerializer.Deserialize(data.Data, type).AssertNull();
            await DeleteOutbox(data.Id, cancellationToken);
            try
            {
                await _publisher.Publish(deserialized, cancellationToken);
                await _outboxRepository.SaveChanges(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "During outbox publishing got an error");
                try
                {
                    await _outboxRepository.DiscardChanges(cancellationToken);
                }
                catch (Exception ex2)
                {
                    _logger.LogCritical(ex2, "During rollback outbox publishing got an error");
                    break;
                }
            }
        }
    }

    private async Task DeleteOutbox(Guid id, CancellationToken cancellationToken) =>
        await _outboxRepository.Delete(id, cancellationToken);

    private async Task<OutboxEntity[]> GetNext(CancellationToken cancellationToken) =>
        (await _outboxRepository.GetNext(cancellationToken)).ToArray();
}