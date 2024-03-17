using System.Text.Json;
using Confluent.Kafka;

namespace EasyBus.Transports.Kafka.Options;

public sealed class KafkaMessagePublisherOptions<T>
{
    public Action<Message<Null, string>> MessageInterceptor { get; set; } = x => { };
    public Func<T, Message<Null, string>>? MessageFactory { get; set; }
    public Func<T, string> MessageSerializer { get; set; } = x => JsonSerializer.Serialize(x);
    public string MessageQueueName { get; set; } = default!;
    public string Topic { get; set; } = default!;
}