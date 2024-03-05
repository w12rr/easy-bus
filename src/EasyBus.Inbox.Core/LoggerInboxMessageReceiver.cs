using Microsoft.Extensions.Logging;

namespace EasyBus.Inbox.Core;

public class LoggerInboxMessageReceiver<T> : IInboxMessageReceiver<T>
{
    private readonly ILogger<LoggerInboxMessageReceiver<T>> _logger;

    public LoggerInboxMessageReceiver(ILogger<LoggerInboxMessageReceiver<T>> logger)
    {
        _logger = logger;
    }

    public Task<InboxMessageState> Receive(T @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receiving event {@Event}", @event);
        return Task.FromResult(InboxMessageState.NotReceived);
    }

    public async Task<InboxMessageState> Receive(object @event, CancellationToken cancellationToken)
    {
        return await Receive((T)@event, cancellationToken);
    }
}