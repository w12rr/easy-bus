using Confluent.Kafka;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Transports.Kafka.DependencyInjection;

public class KafkaMessagePublisherOptionsConfiguration<T>
{
    public IServiceCollection Services { get; set; }

    public KafkaMessagePublisherOptionsConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public void SetPublisher(string messageQueueName, string topic)
    {
        Services.PostConfigure<KafkaMessagePublisherOptions<T>>(x =>
        {
            x.Topic = topic;
            x.MessageQueueName = messageQueueName;
        });
    }

    public void SetMessageInterceptor(Action<Message<Null, string>> messageInterceptor)
    {
        Services.PostConfigure<KafkaMessagePublisherOptions<T>>(x =>
        {
            x.MessageInterceptor = messageInterceptor;
        });
    }

    public void SetMessageFactory(Func<T, Message<Null, string>> messageFactory)
    {
        Services.PostConfigure<KafkaMessagePublisherOptions<T>>(x =>
        {
            x.MessageFactory = messageFactory;
        });
    }

    public void SetMessageSerializer(Func<T, string> messageSerializer)
    {
        Services.PostConfigure<KafkaMessagePublisherOptions<T>>(x =>
        {
            x.MessageSerializer = messageSerializer;
        });
    }
}