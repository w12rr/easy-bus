using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.Kafka.ConnectionStore;
using EasyBus.Transports.Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyBus.Transports.Kafka.Publishing;

public class KafkaInfrastructurePublisher<T> : IInfrastructurePublisher<T>
{
    private readonly string _topic;
    private readonly string _mqName;
    private readonly IProducersStore _producersStore;
    private readonly KafkaMessagePublisherOptions<T> _messagePublisherOptions;

    public KafkaInfrastructurePublisher(string topic, string mqName, IProducersStore producersStore,
        KafkaMessagePublisherOptions<T> messagePublisherOptions)
    {
        _topic = topic;
        _mqName = mqName;
        _producersStore = producersStore;
        _messagePublisherOptions = messagePublisherOptions;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var publisher = _producersStore.GetCachedByName(_mqName);
        Console.WriteLine($"Publishing on topic: {_topic}");
        await publisher.ProduceAsync(_topic, CreateMessage(@event), cancellationToken);
    }

    private Message<Null, string> CreateMessage(T @event)
    {
        var message = _messagePublisherOptions.MessageFactory?.Invoke(@event) ?? new Message<Null, string>
        {
            Value = _messagePublisherOptions.MessageSerializer(@event)
        };
        _messagePublisherOptions.MessageInterceptor(message);
        return message;
    }

    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        await Publish((T)@event, cancellationToken);
    }
}