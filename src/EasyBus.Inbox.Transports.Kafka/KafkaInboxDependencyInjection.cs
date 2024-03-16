using EasyBus.Inbox.Core;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.Kafka;

public static class KafkaInboxDependencyInjection
{
    public static IInboxConfiguration<KafkaReceiverPostConfiguration<T>, T> SetInbox<T>(
        this KafkaReceiverPostConfiguration<T> conf)
    {
        conf.SetFuncHandler(
            async (sp, _, @event) => await sp.GetRequiredService<IInboxMessageIntoDbWriter<T>>()
                .WriteIntoDb(@event, CancellationToken.None));
        return new KafkaInboxConfiguration<T>(conf);
    }
}