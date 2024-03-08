using EasyBus.Inbox.Core;
using EasyBus.Transports.AzureServiceBus.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.AzureServiceBus;

public static class AzureServiceBusInboxDependencyInjection
{
    public static IInboxConfiguration<AzureServiceBusReceiverPostConfiguration<T>, T> SetInbox<T>(
        this AzureServiceBusReceiverPostConfiguration<T> conf)
    {
        conf.SetFuncHandler(
            async (sp, @event) => await sp.GetRequiredService<IInboxMessageIntoDbWriter<T>>()
                .WriteIntoDb(@event, CancellationToken.None));
        return new AzureServiceBusInboxConfiguration<T>(conf);
    }
}