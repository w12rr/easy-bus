using Confluent.Kafka;
using EasyBus.Transports.Kafka.Receiving;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Transports.Kafka.DependencyInjection;

public class KafkaReceiverConfiguration<T>
{
    public IServiceCollection Services;

    public KafkaReceiverConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public void SetFuncHandler(Func<IServiceProvider, ConsumeResult<Ignore, string>, T, Task> onSuccess)
    {
        Services.AddScoped<IKafkaMessageHandler<T>>(sp =>
        {
            return new FuncKafkaMessageHandler<T>(async (args, @event) => await onSuccess(sp, args, @event));
        });
    }

    public void SetHandler<THandler>()
        where THandler : class, IKafkaMessageHandler<T>
    {
        Services.AddScoped<IKafkaMessageHandler<T>, THandler>();
    }

    public void SetConsumer(string messageQueue, string topic, string consumerGroup)
    {
        Services.PostConfigure<KafkaInfrastructureMessageReceiverOptions<T>>(x =>
        {
            x.TopicName = topic;
            x.ConsumerGroup = consumerGroup;
            x.MessageQueueName = messageQueue;
        });
    }
}