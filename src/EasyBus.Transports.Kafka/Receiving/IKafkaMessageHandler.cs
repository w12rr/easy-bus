using Confluent.Kafka;

namespace EasyBus.Transports.Kafka.Receiving;

public interface IKafkaMessageHandler<in T>
{
    Task Handle(ConsumeResult<Ignore, string> message, T @event);
}