using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.Kafka.Receiving;

public class LoggerKafkaMessageHandler<T> : IKafkaMessageHandler<T>
{
    private readonly ILogger<LoggerKafkaMessageHandler<T>> _logger;

    public LoggerKafkaMessageHandler(ILogger<LoggerKafkaMessageHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task Handle(ConsumeResult<Ignore, string> message, T @event)
    {
        _logger.LogInformation("Got message: {Topic} {@Event}", message.Topic, @event);
        return Task.CompletedTask;
    }
}