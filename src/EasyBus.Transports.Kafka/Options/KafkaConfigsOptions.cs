namespace EasyBus.Transports.Kafka.Options;

public sealed class KafkaConfigsOptions
{
    public IDictionary<string, KafkaConnectionOptions> Connections { get; } = new Dictionary<string, KafkaConnectionOptions>();
}