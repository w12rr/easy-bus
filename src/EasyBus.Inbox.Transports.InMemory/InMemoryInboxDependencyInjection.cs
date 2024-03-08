using EasyBus.Inbox.Core;
using EasyBus.Transports.InMemory.Receivers;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.InMemory;

public static class InMemoryInboxDependencyInjection
{
    public static IInboxConfiguration<InMemoryReceiverPostConfiguration<T>, T> SetInbox<T>(
        this InMemoryReceiverPostConfiguration<T> conf)
    {
        conf.SetFuncHandler(
            async (sp, @event) => await sp.GetRequiredService<IInboxMessageIntoDbWriter<T>>()
                .WriteIntoDb(@event, CancellationToken.None));
        return new InMemoryInboxConfiguration<T>(conf);
    }
}