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
    private readonly IProducersStore _producersStore;
    private readonly KafkaMessagePublisherOptions<T> _messagePublisherOptions;

    public KafkaInfrastructurePublisher(IProducersStore producersStore,
        IOptions<KafkaMessagePublisherOptions<T>> messagePublisherOptions)
    {
        _producersStore = producersStore;
        _messagePublisherOptions = messagePublisherOptions.Value;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var publisher = _producersStore.GetCachedByName(_messagePublisherOptions.MessageQueueName);
        Console.WriteLine($"Publishing on topic: {_messagePublisherOptions.Topic}");
        await publisher.ProduceAsync(_messagePublisherOptions.Topic, CreateMessage(@event), cancellationToken);
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