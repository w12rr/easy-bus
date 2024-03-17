using EasyBus.Inbox.Core;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Infrastructure;

public static class InboxDependencyInjectionExtensions
{
    public static void AddInboxMessageConsumer(this ReceiverConfiguration receiver, Action<InboxConfiguration> inboxConfiguration)
    {
        receiver.Services.AddHostedService<InboxConsumerBackgroundService>();
        receiver.Services.AddHostedService<InboxOldMessagesDeleterBackgroundService>();
        receiver.Services.AddTransient<IInboxRepository, InboxRepository>();
        receiver.Services.AddTransient<IConsumingRunner, ConsumingRunner>();
        receiver.Services.AddTransient<IMissingOutboxTableCreator, MissingOutboxTableCreator>();
        receiver.Services.AddTransient<IOldProcessedMessagesDeleter, OldProcessedMessagesDeleter>();
        inboxConfiguration(new InboxConfiguration(receiver.Services));
    }
}