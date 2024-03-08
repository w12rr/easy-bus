using Confluent.Kafka;

namespace EasyBus.Kafka.Receiving;

public interface IKafkaMessageHandler<in T>
{
    Task Handle(ConsumeResult<Ignore, string> message, T @event);
}