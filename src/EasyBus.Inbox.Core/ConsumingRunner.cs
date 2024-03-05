using System.Text.Json;
using EasyBus.Core.Helpers;

namespace EasyBus.Inbox.Core;

public class ConsumingRunner : IConsumingRunner
{
    private readonly IReadOnlyCollection<IInboxMessageReceiver> _messageReceivers;
    private readonly IInboxRepository _inboxRepository;

    public ConsumingRunner(IReadOnlyCollection<IInboxMessageReceiver> messageReceivers,
        IInboxRepository inboxRepository)
    {
        _messageReceivers = messageReceivers;
        _inboxRepository = inboxRepository;
    }
    
    public async Task Run(CancellationToken cancellationToken)
    {
        var next = await GetNext(cancellationToken); //todo keep order of correlation id
        await ProcessNext(next, cancellationToken);
    }
    
    private async Task ProcessNext(IEnumerable<InboxEntity> next, CancellationToken cancellationToken)
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
                await UpdatePickupDate(message, cancellationToken);
            }

            if (inboxState is InboxMessageState.Received)
            {
                await SetReceived(message.Id, cancellationToken);
            }

            throw new Exception("Invalid inbox action");
        }
    }

    private async Task UpdatePickupDate(InboxEntity entity, CancellationToken cancellationToken)
    {
        var nextTries = entity.TriesCount + 1;
        var nextPickupDate = entity.InsertDate.AddSeconds(Math.Pow(2, entity.TriesCount));
        await _inboxRepository.UpdateTries(nextTries, nextPickupDate, entity.Id, cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
    }

    private async Task SetReceived(Guid id, CancellationToken cancellationToken)
    {
        await _inboxRepository.SetProcessed(id, cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
    }

    private async Task<IEnumerable<InboxEntity>> GetNext(CancellationToken cancellationToken) =>
        await _inboxRepository.GetUnprocessedMessages(cancellationToken);
}