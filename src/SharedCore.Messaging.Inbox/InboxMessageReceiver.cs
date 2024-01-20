using SharedCore.Messaging.Core.Definitions;
using SharedCore.Messaging.Core.Receiving;
using SharedCore.Messaging.Inbox.Persistence;
using SharedCore.Messaging.Inbox.Persistence.Entities;

namespace SharedCore.Messaging.Inbox;

public class InboxMessageReceiver : IMessageReceiver
{
    private readonly IInboxRepository _inboxRepository;

    public InboxMessageReceiver(IInboxRepository inboxRepository)
    {
        _inboxRepository = inboxRepository;
    }
    
    public async Task Receive(string message, string correlationId, string messageId, IEventReceiverDefinition definition, CancellationToken cancellationToken)
    {
        var inBoxEntity = new InboxMessage
        {
            CorrelationId = correlationId,
            DefinitionId = definition.GetDefinitionId(),
            Message = message,
            MessageId = messageId,
        };
        
        _inboxRepository.Add(inBoxEntity);

        await _inboxRepository.SaveChanges(cancellationToken);
    }
}