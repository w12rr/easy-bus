using EasyBus.Inbox.Core;
using EasyBus.Transports.AzureServiceBus.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.AzureServiceBus;

public class AzureServiceBusInboxConfiguration<T> : IInboxConfiguration<AzureServiceBusReceiverPostConfiguration<T>, T>
{
    private readonly AzureServiceBusReceiverPostConfiguration<T> _conf;

    public AzureServiceBusInboxConfiguration(AzureServiceBusReceiverPostConfiguration<T> conf)
    {
        _conf = conf;
    }

    public AzureServiceBusReceiverPostConfiguration<T> SetInboxFuncHandler(
        Func<IServiceProvider, T, CancellationToken, Task<InboxMessageState>> handler)
    {
        _conf.Services.AddScoped<IInboxMessageReceiver<T>>(
            sp => new FuncInboxMessageReceiver<T>(
                async (@event, cancellationToken) => await handler(sp, @event, cancellationToken)));
        return _conf;
    }

    public AzureServiceBusReceiverPostConfiguration<T> SetInboxHandler<THandler>()
        where THandler : class, IInboxMessageReceiver<T>
    {
        _conf.Services.AddScoped<IInboxMessageReceiver<T>, THandler>();
        return _conf;
    }
}