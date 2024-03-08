using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.Kafka.Publishing;

public class KafkaInfrastructurePublisher<T> : IInfrastructurePublisher<T>
{
    private readonly KafkaConnectionOptions _kafkaConnectionOptions;
    private readonly string _topic;
    private readonly ILogger<KafkaInfrastructurePublisher<T>> _logger;

    public KafkaInfrastructurePublisher(KafkaConnectionOptions kafkaConnectionOptions, string topic,
        ILogger<KafkaInfrastructurePublisher<T>> logger)
    {
        _kafkaConnectionOptions = kafkaConnectionOptions;
        _topic = topic;
        _logger = logger;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        using var publisher = CreatePublisher();
        await publisher.ProduceAsync(_topic, CreateMessage(@event), cancellationToken);
    }

    private static Message<Null, string> CreateMessage(T @event) => new()
    {
        Value = JsonSerializer.Serialize(JsonSerializer.Serialize(@event))
    };

    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        await Publish((T)@event, cancellationToken);
    }

    private IProducer<Null, string> CreatePublisher()
    {
        return new ProducerBuilder<Null, string>(GetConfig()).SetErrorHandler(ErrorHandler).Build();
    }

    private IEnumerable<KeyValuePair<string, string>> GetConfig() => new ProducerConfig
    {
        SecurityProtocol = _kafkaConnectionOptions.SecurityProtocol,
        SaslMechanism = _kafkaConnectionOptions.SaslMechanism,
        BootstrapServers = string.Join(",", _kafkaConnectionOptions.BootstrapServers),
        SaslPassword = _kafkaConnectionOptions.SaslPassword,
        SslKeyPassword = _kafkaConnectionOptions.SslKeyPassword,
        SslKeystorePassword = _kafkaConnectionOptions.SslKeystorePassword,
        SaslUsername = _kafkaConnectionOptions.SaslUsername
    };

    private void ErrorHandler(IProducer<Null, string> consumer, Error error)
    {
        _logger.LogError("Got error during publishing message {Error} {ConsumerName}", error, consumer.Name);
    }
}