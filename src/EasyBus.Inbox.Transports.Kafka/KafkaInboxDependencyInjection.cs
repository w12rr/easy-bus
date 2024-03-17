using EasyBus.Inbox.Core;
using EasyBus.Transports.Kafka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Transports.Kafka;

public static class KafkaInboxDependencyInjection
{
    public static IInboxConfiguration<KafkaReceiverPostConfiguration<T>, T> SetInbox<T>(
        this KafkaReceiverPostConfiguration<T> conf,
        Func<T, string> messageIdProvider)
    {
        conf.Services.AddTransient<IInboxMessageIntoDbWriter<T>>(
            sp =>
            {
                var repository = sp.GetRequiredService<IInboxRepository>();
                return new InboxMessageIntoDbWriter<T>(messageIdProvider, repository);
            });
        conf.SetFuncHandler(
            async (sp, _, @event) => await sp.GetRequiredService<IInboxMessageIntoDbWriter<T>>()
                .WriteIntoDb(@event, CancellationToken.None));
        return new KafkaInboxConfiguration<T>(conf);
    }
}