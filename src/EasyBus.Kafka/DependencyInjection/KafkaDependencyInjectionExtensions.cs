using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Kafka.Options;
using EasyBus.Kafka.Publishing;
using EasyBus.Kafka.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Kafka.DependencyInjection;

public static class KafkaDependencyInjectionExtensions
{
    public static void AddKafka(this MessageQueueConfiguration mqConfiguration, string name,
        Action<KafkaConnectionOptions> asbOptionsConfig)
    {
        mqConfiguration.Services.AddOptions<KafkaOptions>();
        mqConfiguration.Services.PostConfigure<KafkaOptions>(opt =>
        {
            var asbOpt = new KafkaConnectionOptions();
            asbOptionsConfig(asbOpt);
            opt.Connections.Add(name, asbOpt);
        });
    }

    public static void AddAzureServiceBusEventPublisher<T>(this PublisherConfiguration configuration,
        string mqName,
        string topic)
    {
        configuration.Services.AddScoped<IInfrastructurePublisher<T>, KafkaInfrastructurePublisher<T>>(
            sp =>
            {
                var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value.Connections[mqName];
                var logger = sp.GetRequiredService<ILogger<KafkaInfrastructurePublisher<T>>>();
                return new KafkaInfrastructurePublisher<T>(options, topic, logger);
            });
    }

    public static KafkaReceiverPostConfiguration<T> AddKafkaReceiver<T>(this ReceiverConfiguration configuration,
        string mqName,
        string topicName,
        string consumerGroup)
    {
        configuration.Services.AddScoped<IKafkaMessageHandler<T>, LoggerKafkaMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, KafkaInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value.Connections[mqName];
            var logger = sp.GetRequiredService<ILogger<KafkaInfrastructureReceiver<T>>>();
            var handler = sp.GetRequiredService<IKafkaMessageHandler<T>>();
            return new KafkaInfrastructureReceiver<T>(options, handler, topicName, consumerGroup, logger);
        });
        return new KafkaReceiverPostConfiguration<T>(configuration.Services);
    }
}