using EasyBus.Inbox.Core;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.Kafka;

public class KafkaInboxConfiguration<T> : IInboxConfiguration<KafkaReceiverPostConfiguration<T>, T>
{
    private readonly KafkaReceiverPostConfiguration<T> _conf;

    public KafkaInboxConfiguration(KafkaReceiverPostConfiguration<T> conf)
    {
        _conf = conf;
    }

    public KafkaReceiverPostConfiguration<T> SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageState>> handler)
    {
        _conf.Services.AddScoped<IInboxMessageReceiver>(
            sp => new FuncInboxMessageReceiver<T>(
                async (@event, cancellationToken) => await handler(sp, @event, cancellationToken)));
        return _conf;
    }

    public KafkaReceiverPostConfiguration<T> SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>
    {
        _conf.Services.AddScoped<IInboxMessageReceiver, THandler>();
        return _conf;
    }
}