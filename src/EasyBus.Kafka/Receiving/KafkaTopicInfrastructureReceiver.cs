using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.Helpers;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Kafka.Options;
using Microsoft.Extensions.Logging;

namespace EasyBus.AzureServiceBus.Receiving;

public class KafkaTopicInfrastructureReceiver<T> : IInfrastructureReceiver, IDisposable
{
    private readonly KafkaConnectionOptions _kafkaConnectionOptions;
    private readonly IKafkaMessageHandler<T> _messageHandler;
    private readonly string _topicName;
    private readonly string _consumerGroupName;
    private readonly ILogger<KafkaTopicInfrastructureReceiver<T>> _logger;
    private IConsumer<Ignore, string>? _consumer;


    public KafkaTopicInfrastructureReceiver(KafkaConnectionOptions kafkaConnectionOptions,
        IKafkaMessageHandler<T> messageHandler,
        string topicName,
        string consumerGroupName,
        ILogger<KafkaTopicInfrastructureReceiver<T>> logger)
    {
        _kafkaConnectionOptions = kafkaConnectionOptions;
        _messageHandler = messageHandler;
        _topicName = topicName;
        _consumerGroupName = consumerGroupName;
        _logger = logger;
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        _consumer = new ConsumerBuilder<Ignore, string>(GetConfig())
            .SetErrorHandler(ErrorHandler)
            .Build();

        _consumer.Subscribe(_topicName);

        _ = Task.Run(async () =>
        {
            var message = _consumer.Consume();
            var @event = JsonSerializer.Deserialize<T>(message.Message.Value).AssertNull();
            await _messageHandler.Handle(message.AssertNull(), @event);
        }, cancellationToken);
    }

    private IEnumerable<KeyValuePair<string, string>> GetConfig() => new ConsumerConfig
    {
        SecurityProtocol = _kafkaConnectionOptions.SecurityProtocol,
        SaslMechanism = _kafkaConnectionOptions.SaslMechanism,
        BootstrapServers = string.Join(",", _kafkaConnectionOptions.BootstrapServers),
        SaslPassword = _kafkaConnectionOptions.SaslPassword,
        SslKeyPassword = _kafkaConnectionOptions.SslKeyPassword,
        SslKeystorePassword = _kafkaConnectionOptions.SslKeystorePassword,
        SaslUsername = _kafkaConnectionOptions.SaslUsername,
        AllowAutoCreateTopics = true,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        GroupId = _consumerGroupName
    };

    private void ErrorHandler(IConsumer<Ignore, string> consumer, Error error)
    {
        _logger.LogError("Got error during publishing message {Error} {ConsumerName}", error, consumer.Name);
    }

    public void Dispose()
    {
        _consumer?.Dispose();
    }
}