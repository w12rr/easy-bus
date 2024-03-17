using System.Text.Json;
using EasyBus.Core.Helpers;

namespace EasyBus.Inbox.Core;

public class InboxMessageIntoDbWriter<T> : IInboxMessageIntoDbWriter<T>
{
    private readonly IMessageIdProvider<T> _messageIdProvider;
    private readonly IInboxRepository _inboxRepository;

    public InboxMessageIntoDbWriter(IMessageIdProvider<T> messageIdProvider, IInboxRepository inboxRepository)
    {
        _messageIdProvider = messageIdProvider;
        _inboxRepository = inboxRepository;
    }

    public async Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken)
    {
        var messageId = _messageIdProvider.GetId(@event);
        var type = typeof(T).AssemblyQualifiedName.AssertNull();
        var json = JsonSerializer.Serialize(@event);
        await _inboxRepository.Insert(messageId, type, json, cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
        return true;
    }
}