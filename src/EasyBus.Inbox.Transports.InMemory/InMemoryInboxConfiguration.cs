using EasyBus.Inbox.Core;
using EasyBus.Transports.InMemory.Receivers;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.InMemory;

public class InMemoryInboxConfiguration<T> : IInboxConfiguration<InMemoryReceiverPostConfiguration<T>, T>
{
    private readonly InMemoryReceiverPostConfiguration<T> _conf;

    public InMemoryInboxConfiguration(InMemoryReceiverPostConfiguration<T> conf)
    {
        _conf = conf;
    }

    public InMemoryReceiverPostConfiguration<T> SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageState>> handler)
    {
        _conf.Services.AddScoped<IInboxMessageReceiver<T>>(
            sp => new FuncInboxMessageReceiver<T>(
                async (@event, cancellationToken) => await handler(sp, @event, cancellationToken)));
        return _conf;
    }

    public InMemoryReceiverPostConfiguration<T> SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>
    {
        _conf.Services.AddScoped<IInboxMessageReceiver<T>, THandler>();
        return _conf;
    }
}