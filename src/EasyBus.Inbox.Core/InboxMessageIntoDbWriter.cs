using System.Text.Json;
using EasyBus.Core.Helpers;

namespace EasyBus.Inbox.Core;

public class InboxMessageIntoDbWriter<T> : IInboxMessageIntoDbWriter<T>
{
    private readonly Func<T, string> _messageIdProvider;
    private readonly IInboxRepository _inboxRepository;

    public InboxMessageIntoDbWriter(Func<T, string> messageIdProvider, IInboxRepository inboxRepository)
    {
        _messageIdProvider = messageIdProvider;
        _inboxRepository = inboxRepository;
    }

    public async Task<bool> WriteIntoDb(T @event, CancellationToken cancellationToken)
    {
        Console.WriteLine("Consuimg in inbox");
        var messageId = _messageIdProvider(@event);
        var type = typeof(T).AssemblyQualifiedName.AssertNull();
        var json = JsonSerializer.Serialize(@event);
        await _inboxRepository.Insert(messageId, type, json, cancellationToken);
        await _inboxRepository.SaveChanges(cancellationToken);
        return true;
    }
}