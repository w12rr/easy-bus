using Confluent.Kafka;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Transports.Kafka.ConnectionStore;

public class ProducersStore : IProducersStore, IDisposable
{
    private readonly ILogger<ProducersStore> _logger;
    private readonly KafkaOptions _kafkaOptions;

    private readonly Dictionary<string, IProducer<Null, string>> _producerBuilders = new();

    public ProducersStore(IOptions<KafkaOptions> kafkaOptions, ILogger<ProducersStore> logger)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions.Value;
    }

    public IProducer<Null, string> GetCachedByName(string name)
    {
        if (!_producerBuilders.ContainsKey(name)) _producerBuilders.Add(name, CreatePublisher(name));

        return _producerBuilders[name];
    }

    private IProducer<Null, string> CreatePublisher(string name)
    {
        var config = GetConfig(_kafkaOptions.Connections[name]);

        return new ProducerBuilder<Null, string>(config)
            .SetErrorHandler(ErrorHandler).Build();
    }

    private static IEnumerable<KeyValuePair<string, string>> GetConfig(KafkaConnectionOptions options) =>
        new ProducerConfig
        {
            SecurityProtocol = options.SecurityProtocol,
            SaslMechanism = options.SaslMechanism,
            BootstrapServers = string.Join(",", options.BootstrapServers),
            SaslPassword = options.SaslPassword,
            SslKeyPassword = options.SslKeyPassword,
            SslKeystorePassword = options.SslKeystorePassword,
            SaslUsername = options.SaslUsername
        };

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