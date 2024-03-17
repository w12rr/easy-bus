using Confluent.Kafka;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Transports.Kafka.ConnectionStore;

public class ProducersStore : IProducersStore, IDisposable
{
    private readonly ILogger<ProducersStore> _logger;
    private readonly KafkaConfigsOptions _kafkaConfigsOptions;
    private readonly Dictionary<string, IProducer<Null, string>> _producerBuilders = new();

    public ProducersStore(IOptions<KafkaConfigsOptions> kafkaConfigsOptions, ILogger<ProducersStore> logger)
    {
        _logger = logger;
        _kafkaConfigsOptions = kafkaConfigsOptions.Value;
    }

    public IProducer<Null, string> GetCachedByName(string name)
    {
        if (!_producerBuilders.ContainsKey(name)) _producerBuilders.Add(name, CreatePublisher(name));

        return _producerBuilders[name];
    }

    private IProducer<Null, string> CreatePublisher(string name)
    {
        var kafkaConnectionOptions = _kafkaConfigsOptions.Connections[name];
        var config = GetConfig(kafkaConnectionOptions);

        var producerBuilder = kafkaConnectionOptions.ProducerBuilderFactory(config).SetErrorHandler(ErrorHandler);
        kafkaConnectionOptions.ProducerBuilderInterceptor(producerBuilder);
        return producerBuilder.Build();
    }

    private static ProducerConfig GetConfig(KafkaConnectionOptions options)
    {
        var producerConfig = new ProducerConfig
        {
            SecurityProtocol = options.SecurityProtocol,
            SaslMechanism = options.SaslMechanism,
            BootstrapServers = string.Join(",", options.BootstrapServers),
            SaslPassword = options.SaslPassword,
            SslKeyPassword = options.SslKeyPassword,
            SslKeystorePassword = options.SslKeystorePassword,
            SaslUsername = options.SaslUsername,
        };
        options.ProducerConfigInterceptor(producerConfig);
        return producerConfig;
    }

    private void ErrorHandler(IProducer<Null, string> consumer, Error error)
    {
        _logger.LogError("Got error during publishing message {Error} {ConsumerName}", error, consumer.Name);
    }

    public void Dispose()
    {
        foreach (var producer in _producerBuilders)
        {
            producer.Value.Dispose();
        }
    }
}