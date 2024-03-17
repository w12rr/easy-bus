using EasyBus.Inbox.Core;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.Kafka;

public static class KafkaInboxDependencyInjection
{
    public static void UseInbox<T>(
        this KafkaReceiverPostConfiguration<T> conf,
        Action<KafkaInboxConfiguration<T>> configAction)
    {

        conf.Services.AddTransient<IInboxMessageIntoDbWriter<T>, InboxMessageIntoDbWriter<T>>();
        conf.SetFuncHandler(
            async (sp, _, @event) => await sp.GetRequiredService<IInboxMessageIntoDbWriter<T>>()
                .WriteIntoDb(@event, CancellationToken.None));
        configAction(new KafkaInboxConfiguration<T>(conf));
    }
}