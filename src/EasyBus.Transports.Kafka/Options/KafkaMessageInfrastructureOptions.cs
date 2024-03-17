namespace EasyBus.Transports.Kafka.Options;

public sealed class KafkaMessagePublisherInfrastructureOptions<T>
{
    public string Topic { get; set; } = default!;
    public string MqName { get; set; } = default!;
}