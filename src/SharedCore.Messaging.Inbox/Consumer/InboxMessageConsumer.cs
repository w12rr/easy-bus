using System.Collections.Immutable;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedCore.Features.DistributedLock.Core;
using SharedCore.Messaging.Core.Definitions;
using SharedCore.Messaging.Inbox.Persistence;
using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox.Consumer;

public sealed class InboxMessageConsumer : IInboxMessageConsumer
{
    private readonly IInboxRepository _inboxRepository;
    private readonly IMediator _mediator;
    private readonly IDistributedLock _distributedLock;
    private readonly ILogger<InboxMessageConsumer> _logger;
    private readonly ImmutableSortedDictionary<string, IEventReceiverDefinition> _definitions;

    public InboxMessageConsumer(IInboxRepository inboxRepository,
        IEnumerable<IEventReceiverDefinition> definitions,
        IMediator mediator,
        IDistributedLock distributedLock,
        ILogger<InboxMessageConsumer> logger)
    {
        _inboxRepository = inboxRepository;
        _mediator = mediator;
        _distributedLock = distributedLock;
        _logger = logger;
        _definitions = definitions.ToImmutableSortedDictionary(x => x.GetDefinitionId(), x => x);
    }

    public async Task Consume(CancellationToken cancellationToken) 
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await StartConsumingSession(cancellationToken);
                await Task.Delay(Variables.SleepTimeBetweenConsumingSessionsInMilliseconds, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Got error during consuming");
                await Task.Delay(Variables.SleepTimeBetweenErrorConsumingSessionsInMilliseconds, cancellationToken);
            }
        }
    }

    private async Task StartConsumingSession(CancellationToken cancellationToken)
    {
        var messages = await _inboxRepository.GetIbBoxMessages(cancellationToken);
        var failedCorrelationIds = new List<string>();

        while (messages.Any())
        {
            foreach (var message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message.CorrelationId)
                    && failedCorrelationIds.Contains(message.CorrelationId)) continue;

                await CreateLock(
                    message,
                    async () =>
                    {
                        try
                        {
                            var definition = _definitions[message.DefinitionId];
                            var notification = definition.GetNotification(message.Message);
                            await _mediator.Publish(notification, cancellationToken);
                            _inboxRepository.Remove(message);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Got error during consuming {@Message}", message);
                            if (!string.IsNullOrWhiteSpace(message.CorrelationId))
                            {
                                failedCorrelationIds.Add(message.CorrelationId);
                            }

                            message.NextReadAt = DateTimeOffset.Now.AddSeconds(
                                Variables.ConsumingErrorSleepTimeInSeconds);
                        }
                        finally
                        {
                            await _inboxRepository.SaveChanges(cancellationToken);
                        }
                    },
                    () =>
                    {
                        if (!string.IsNullOrWhiteSpace(message.CorrelationId))
                        {
                            failedCorrelationIds.Add(message.CorrelationId);
                        }
                    });
            }

            messages = await _inboxRepository.GetIbBoxMessages(cancellationToken);
            failedCorrelationIds.Clear();
        }
    }

    private async Task CreateLock(InboxMessage message, Func<Task> onLock, Action onError)
    {
        if (string.IsNullOrWhiteSpace(message.CorrelationId))
        {
            await onLock();
            return;
        }

        try
        {
            await _distributedLock.CreateLock(Variables.LockName(message.DefinitionId, message.CorrelationId), onLock);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot get lock for {@Message}", message);
            onError();
        }
    }
}