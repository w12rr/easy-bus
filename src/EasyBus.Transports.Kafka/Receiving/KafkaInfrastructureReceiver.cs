using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.Helpers;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Transports.Kafka.Receiving;

public class KafkaInfrastructureReceiver<T> : IInfrastructureReceiver, IAsyncDisposable
{
    private readonly KafkaConnectionOptions _kafkaConnectionOptions;
    private readonly IKafkaMessageHandler<T> _messageHandler;
    private readonly ILogger<KafkaInfrastructureReceiver<T>> _logger;
    private IConsumer<Ignore, string>? _consumer;
    private Task? _subscriberTask;
    private CancellationTokenSource? _subscriberCts;
    private bool _taskFinished;
    private readonly KafkaInfrastructureMessageReceiverOptions<T> _kafkaInfrastructureMessageReceiverOptions;

    public KafkaInfrastructureReceiver(IOptions<KafkaConfigsOptions> kafkaOptions,
        IKafkaMessageHandler<T> messageHandler,
        ILogger<KafkaInfrastructureReceiver<T>> logger,
        IOptions<KafkaInfrastructureMessageReceiverOptions<T>> kafkaInfrastructureMessageReceiverOptions)
    {
        _kafkaConnectionOptions = kafkaOptions.Value.Connections[kafkaInfrastructureMessageReceiverOptions.Value.MessageQueueName];
        _messageHandler = messageHandler;
        _logger = logger;
        _kafkaInfrastructureMessageReceiverOptions = kafkaInfrastructureMessageReceiverOptions.Value;
    }

    public Task StartReceiving(CancellationToken cancellationToken)
    {
        _consumer = GetConsumer();

        _consumer.Subscribe(_kafkaInfrastructureMessageReceiverOptions.TopicName);

        _subscriberCts = new CancellationTokenSource();
        _subscriberTask = Task.Run(async () =>
        {
            while (!_subscriberCts.IsCancellationRequested) //todo ct token
            {
                try
                {
                    var message = _consumer.Consume(_subscriberCts.Token);
                    _kafkaInfrastructureMessageReceiverOptions.BeforeMessageConsume(message);
                    var @event = _kafkaInfrastructureMessageReceiverOptions.MessageDeserializer(message.Message.Value);
                    _kafkaInfrastructureMessageReceiverOptions.AfterDeserializing(message, @event);
                    await _messageHandler.Handle(message.AssertNull(), @event);
                    _kafkaInfrastructureMessageReceiverOptions.AfterMessageConsume(message, @event);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            _taskFinished = true;
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private IConsumer<Ignore, string> GetConsumer()
    {
        var config = GetConfig();
        var builder = _kafkaConnectionOptions.ConsumerBuilderFactory(config).SetErrorHandler(ErrorHandler);
        _kafkaConnectionOptions.ConsumerBuilderInterceptor(builder);
        return builder.Build();
    }

    private ConsumerConfig GetConfig()
    {
        var config = new ConsumerConfig
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
            GroupId = _kafkaInfrastructureMessageReceiverOptions.ConsumerGroup
        };
        _kafkaConnectionOptions.ConsumerConfigInterceptor(config);
        return config;
    }

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