using EasyBus.Inbox.Core;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.Kafka;

public class KafkaInboxConfiguration<T> : IInboxConfiguration<T>
{
    private readonly KafkaReceiverConfiguration<T> _conf;

    public KafkaInboxConfiguration(KafkaReceiverConfiguration<T> conf)
    {
        _conf = conf;
    }

    public void SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageState>> handler)
    {
        _conf.Services.AddScoped<IInboxMessageReceiver>(
            sp => new FuncInboxMessageReceiver<T>(
                async (@event, cancellationToken) => await handler(sp, @event, cancellationToken)));
    }

    public void SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>
    {
        _conf.Services.AddScoped<IInboxMessageReceiver, THandler>();
    }

    public void SetMessageIdProvider(Func<T, string> idProvider)
    {
        _conf.Services.AddTransient<IMessageIdProvider<T>>(_ => new MessageIdProvider<T>(idProvider));
    }
}