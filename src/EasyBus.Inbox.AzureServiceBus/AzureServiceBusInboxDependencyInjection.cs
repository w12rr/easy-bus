using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.DependencyInjection;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.Inbox.Core;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.AzureServiceBus;

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