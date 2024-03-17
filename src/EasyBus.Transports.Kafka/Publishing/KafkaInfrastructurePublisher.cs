using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.Kafka.ConnectionStore;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.Kafka.Publishing;

public class KafkaInfrastructurePublisher<T> : IInfrastructurePublisher<T>
{
    private readonly string _topic;
    private readonly string _mqName;
    private readonly IProducersStore _producersStore;

    public KafkaInfrastructurePublisher(string topic, string mqName, IProducersStore producersStore)
    {
        _topic = topic;
        _mqName = mqName;
        _producersStore = producersStore;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var publisher = _producersStore.GetCachedByName(_mqName);
        Console.WriteLine($"Publishing on topic: {_topic}");
        await publisher.ProduceAsync(_topic, CreateMessage(@event), cancellationToken);
    }

    private static Message<Null, string> CreateMessage(T @event) => new()
    {
        Value = JsonSerializer.Serialize(@event)
    };

    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        await Publish((T)@event, cancellationToken);
    }
}