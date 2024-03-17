using Confluent.Kafka;

namespace EasyBus.Transports.Kafka.Options;

public sealed class KafkaConnectionOptions
{
    public string[] BootstrapServers { get; set; } = Array.Empty<string>();
    public SaslMechanism? SaslMechanism { get; set; } = Confluent.Kafka.SaslMechanism.Plain;
    public SecurityProtocol? SecurityProtocol { get; set; } = Confluent.Kafka.SecurityProtocol.Plaintext;
    public string SaslPassword { get; set; } = string.Empty;
    public string SslKeyPassword { get; set; } = string.Empty;
    public string SslKeystorePassword { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    
    public Action<ProducerConfig> ProducerConfigInterceptor { get; set; } = x => { };
    public Action<ProducerBuilder<Null, string>> ProducerBuilderInterceptor { get; set; } = x => { };
    public Func<ProducerConfig, ProducerBuilder<Null, string>> ProducerBuilderFactory { get; set; } =
        x => new ProducerBuilder<Null, string>(x);
}