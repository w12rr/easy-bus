using Confluent.Kafka;
using EasyBus.Transports.Kafka.Receiving;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Transports.Kafka.DependencyInjection;

public class KafkaReceiverPostConfiguration<T>
{
    public IServiceCollection Services;

    public KafkaReceiverPostConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public KafkaReceiverPostConfiguration<T> SetFuncHandler(
        Func<IServiceProvider, ConsumeResult<Ignore, string>, T, Task> onSuccess)
    {
        Services.AddScoped<IKafkaMessageHandler<T>>(sp =>
        {
            return new FuncKafkaMessageHandler<T>(async (args, @event) => await onSuccess(sp, args, @event));
        });
        return this;
    }

    public KafkaReceiverPostConfiguration<T> SetHandler<THandler>()
        where THandler : class, IKafkaMessageHandler<T>
    {
        Services.AddScoped<IKafkaMessageHandler<T>, THandler>();
        return this;
    }
}