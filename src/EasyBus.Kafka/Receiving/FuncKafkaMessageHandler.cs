using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyBus.AzureServiceBus.Receiving;

public class FuncKafkaMessageHandler<T> : IKafkaMessageHandler<T>
{
    private readonly ILogger<FuncKafkaMessageHandler<T>> _logger;

    public FuncKafkaMessageHandler(ILogger<FuncKafkaMessageHandler<T>> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ConsumeResult<Ignore, string> message, T @event)
    {
        throw new NotImplementedException();
    }
}