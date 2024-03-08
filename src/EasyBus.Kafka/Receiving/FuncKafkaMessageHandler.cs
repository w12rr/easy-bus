using Confluent.Kafka;

namespace EasyBus.Kafka.Receiving;

public class FuncKafkaMessageHandler<T> : IKafkaMessageHandler<T>
{
    private readonly Func<ConsumeResult<Ignore, string>, T, Task> _handler;

    public FuncKafkaMessageHandler(Func<ConsumeResult<Ignore, string> , T , Task> handler)
    {
        _handler = handler;
    }
    
    public async Task Handle(ConsumeResult<Ignore, string> message, T @event)
    {
        await _handler(message, @event);
    }
}