using Confluent.Kafka;

namespace EasyBus.Transports.Kafka.ConnectionStore;

public interface IProducersStore
{
    IProducer<Null, string> GetCachedByName(string name);
}