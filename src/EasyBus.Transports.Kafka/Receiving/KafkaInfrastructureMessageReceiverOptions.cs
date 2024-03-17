using System.Text.Json;
using Confluent.Kafka;
using EasyBus.Core.Helpers;

namespace EasyBus.Transports.Kafka.Receiving;

public sealed class KafkaInfrastructureMessageReceiverOptions<T>
{
    public Action<ConsumeResult<Ignore, string>> BeforeMessageConsume { get; set; } = x => { };
    public Func<string, T> MessageDeserializer { get; set; } = mess => JsonSerializer.Deserialize<T>(mess).AssertNull();
    public Action<ConsumeResult<Ignore, string>, T> AfterDeserializing { get; set; } = (_, _) => { };
    public Action<ConsumeResult<Ignore, string>, T> AfterMessageConsume { get; set; } = (_, _) => { };
    public string MessageQueueName { get; set; } = default!;
    public string TopicName { get; set; } = default!;
    public string ConsumerGroup { get; set; } = default!;
}