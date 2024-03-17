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
            var kafkaOpt = new KafkaConnectionOptions();
            kafkaOptionsConfig(kafkaOpt);
            opt.Connections.Add(name, kafkaOpt);
        });
        mqConfiguration.Services.AddSingleton<IProducersStore, ProducersStore>();
    }

    public static void AddKafkaEventPublisher<T>(this PublisherConfiguration configuration,
        Action<KafkaMessagePublisherOptionsConfiguration<T>> publisherConfiguration)
    {
        configuration.Services.AddOptions<KafkaMessagePublisherOptions<T>>();
        publisherConfiguration(new KafkaMessagePublisherOptionsConfiguration<T>(configuration.Services));
        configuration.Services.AddScoped<IInfrastructurePublisher, KafkaInfrastructurePublisher<T>>();
    }

    public static void AddKafkaReceiver<T>(this ReceiverConfiguration configuration,
        Action<KafkaReceiverConfiguration<T>>? configAction = default)
    {
        configuration.Services.AddOptions<KafkaInfrastructureMessageReceiverOptions<T>>();
        configuration.Services.AddScoped<IKafkaMessageHandler<T>, LoggerKafkaMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, KafkaInfrastructureReceiver<T>>();
        configAction?.Invoke(new KafkaReceiverConfiguration<T>(configuration.Services));
    }
}