namespace EasyBus.Transports.Kafka.Options;

public sealed class KafkaOptions
{
    public IDictionary<string, KafkaConnectionOptions> Connections { get; } = new Dictionary<string, KafkaConnectionOptions>();
}