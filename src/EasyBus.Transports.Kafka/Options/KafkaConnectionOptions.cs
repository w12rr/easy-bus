using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

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
    public Action<ConsumerConfig> ConsumerConfigInterceptor { get; set; } = x => { };
    public Action<ProducerBuilder<Null, string>> ProducerBuilderInterceptor { get; set; } = x => { };
    public Action<ConsumerBuilder<Ignore, string>> ConsumerBuilderInterceptor { get; set; } = x => { };

    public Func<ProducerConfig, ProducerBuilder<Null, string>> ProducerBuilderFactory { get; set; } =
        x => new ProducerBuilder<Null, string>(x);

    public Func<ConsumerConfig, ConsumerBuilder<Ignore, string>> ConsumerBuilderFactory { get; set; } =
        x => new ConsumerBuilder<Ignore, string>(x); //todo tutaj factory?
}

public sealed class KafkaConnectionOptionsConfigurator
{
    private readonly string _configName;
    public IServiceCollection Services { get; }

    public KafkaConnectionOptionsConfigurator(IServiceCollection services, string configName)
    {
        _configName = configName;
        Services = services;
    }

    public string[] BootstrapServers { get; set; } = Array.Empty<string>();
    public SaslMechanism? SaslMechanism { get; set; } = Confluent.Kafka.SaslMechanism.Plain;
    public SecurityProtocol? SecurityProtocol { get; set; } = Confluent.Kafka.SecurityProtocol.Plaintext;
    public string SaslPassword { get; set; } = string.Empty;
    public string SslKeyPassword { get; set; } = string.Empty;
    public string SslKeystorePassword { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;

    public void SetProducerConfigInterceptor(Action<ProducerConfig> producerConfigInterceptor)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x => x.Connections[_configName])
    }

    public void SetConsumerConfigInterceptor(Action<ConsumerConfig> consumerConfigInterceptor)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x =>
            x.Connections[_configName].ConsumerConfigInterceptor = consumerConfigInterceptor);
    }

    public void SetProducerBuilderInterceptor(Action<ProducerBuilder<Null, string>> producerBuilderInterceptor)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x =>
            x.Connections[_configName].ProducerBuilderInterceptor = producerBuilderInterceptor);
    }

    public void SetConsumerBuilderInterceptor(Action<ConsumerBuilder<Ignore, string>> consumerBuilderInterceptor)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x => x.Connections[_configName])
    }

    public void SetProducerBuilderFactory(Func<ProducerConfig, ProducerBuilder<Null, string>> producerBuilderFactory)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x => x.Connections[_configName])
    }

    public void SetConsumerBuilderFactory(Func<ConsumerConfig, ConsumerBuilder<Ignore, string>> consumerBuilderFactory)
    {
        Services.PostConfigure<KafkaConfigsOptions>(x => x.Connections[_configName])
    }
}