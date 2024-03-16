using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.Helpers;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.Kafka.Receiving;

public class KafkaInfrastructureReceiver<T> : IInfrastructureReceiver, IAsyncDisposable
{
    private readonly KafkaConnectionOptions _kafkaConnectionOptions;
    private readonly IKafkaMessageHandler<T> _messageHandler;
    private readonly string _topicName;
    private readonly string _consumerGroupName;
    private readonly ILogger<KafkaInfrastructureReceiver<T>> _logger;
    private IConsumer<Ignore, string>? _consumer;
    private Task? _subscriberTask;
    private CancellationTokenSource? _subscriberCts;
    private bool _taskFinished;

    public KafkaInfrastructureReceiver(KafkaConnectionOptions kafkaConnectionOptions,
        IKafkaMessageHandler<T> messageHandler,
        string topicName,
        string consumerGroupName,
        ILogger<KafkaInfrastructureReceiver<T>> logger)
    {
        _kafkaConnectionOptions = kafkaConnectionOptions;
        _messageHandler = messageHandler;
        _topicName = topicName;
        _consumerGroupName = consumerGroupName;
        _logger = logger;
    }

    public Task StartReceiving(CancellationToken cancellationToken)
    {
        _consumer = new ConsumerBuilder<Ignore, string>(GetConfig())
            .SetErrorHandler(ErrorHandler)
            .Build();

        _consumer.Subscribe(_topicName);

        _subscriberCts = new CancellationTokenSource();
        _subscriberTask = Task.Run(async () =>
        {
            while (!_subscriberCts.IsCancellationRequested) //todo ct token
            {
                var message = _consumer.Consume(_subscriberCts.Token);
                var @event = JsonSerializer.Deserialize<T>(message.Message.Value).AssertNull();
                await _messageHandler.Handle(message.AssertNull(), @event);
            }

            _taskFinished = true;
        }, cancellationToken);

        return Task.CompletedTask;
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

    public async ValueTask DisposeAsync()
    {
        _subscriberCts?.Cancel();
        while (!_taskFinished) await Task.Delay(10);

        _subscriberCts?.Dispose();
        _consumer?.Unsubscribe();
        _consumer?.Dispose();
        _subscriberTask?.Dispose();
    }
}