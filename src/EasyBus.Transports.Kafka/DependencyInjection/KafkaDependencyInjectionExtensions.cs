using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Transports.Kafka.ConnectionStore;
using EasyBus.Transports.Kafka.Options;
using EasyBus.Transports.Kafka.Publishing;
using EasyBus.Transports.Kafka.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Transports.Kafka.DependencyInjection;

public static class KafkaDependencyInjectionExtensions
{
    public static void AddKafka(this MessageQueueConfiguration mqConfiguration, string name,
        Action<KafkaConnectionOptions> kafkaOptionsConfig)
    {
        mqConfiguration.Services.AddOptions<KafkaConfigsOptions>();
        mqConfiguration.Services.PostConfigure<KafkaConfigsOptions>(opt =>
        {
            var asbOpt = new KafkaConnectionOptions();
            kafkaOptionsConfig(asbOpt);
            opt.Connections.Add(name, asbOpt);
        });
        mqConfiguration.Services.AddSingleton<IProducersStore, ProducersStore>();
    }

    public static void AddKafkaEventPublisher<T>(this PublisherConfiguration configuration,
        Action<KafkaMessagePublisherOptions<T>>? kafkaMessagePublisherOptionsConfig = default)
    {
        configuration.Services.AddOptions<KafkaMessagePublisherOptions<T>>()
            .Configure(x => kafkaMessagePublisherOptionsConfig?.Invoke(x));
        configuration.Services.AddScoped<IInfrastructurePublisher, KafkaInfrastructurePublisher<T>>();
    }

    public static void AddKafkaReceiver<T>(this ReceiverConfiguration configuration,
        string mqName,
        string topicName,
        string consumerGroup,
        Action<KafkaReceiverPostConfiguration<T>>? configAction = default)
    {
        configuration.Services.AddScoped<IKafkaMessageHandler<T>, LoggerKafkaMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, KafkaInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaConfigsOptions>>().Value.Connections[mqName];
            var logger = sp.GetRequiredService<ILogger<KafkaInfrastructureReceiver<T>>>();
            var handler = sp.GetRequiredService<IKafkaMessageHandler<T>>();
            return new KafkaInfrastructureReceiver<T>(options, handler, topicName, consumerGroup, logger);
        });

        configAction?.Invoke(new KafkaReceiverPostConfiguration<T>(configuration.Services));
    }
}