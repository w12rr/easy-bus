namespace EasyBus.Kafka.Options;

public sealed class KafkaOptions
{
    public required IDictionary<string, KafkaConnectionOptions> Connections { get; init; }
}