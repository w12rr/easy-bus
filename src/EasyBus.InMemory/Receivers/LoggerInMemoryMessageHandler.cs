using Microsoft.Extensions.Logging;

namespace EasyBus.InMemory.Receivers;

public class LoggerInMemoryMessageHandler<T> : IInMemoryMessageHandler<T>
{
    private readonly ILogger<LoggerInMemoryMessageHandler<T>> _logger;

    public LoggerInMemoryMessageHandler(ILogger<LoggerInMemoryMessageHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task MessageHandler(T @event)
    {
        _logger.LogInformation("Received event: {EventType} {@EventData}", typeof(T).Name, @event);
        return Task.CompletedTask;
    }
}