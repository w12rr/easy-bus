using Confluent.Kafka;

namespace EasyBus.Transports.Kafka.ConnectionStore;

public sealed class KafkaCreatingOptions
{
    public Action<ProducerConfig> ProducerConfigInterceptor { get; set; } = x => { };
    public Action<ProducerBuilder<Null, string>> ProducerBuilderInterceptor { get; set; } = x => { };
    public Func<ProducerConfig, ProducerBuilder<Null, string>> ProducerBuilderFactory { get; set; } =
        x => new ProducerBuilder<Null, string>(x);
    
}