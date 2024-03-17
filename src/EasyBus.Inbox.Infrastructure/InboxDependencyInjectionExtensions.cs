using EasyBus.Inbox.Core;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Infrastructure;

public static class InboxDependencyInjectionExtensions
{
    public static void AddInboxMessageConsumer(this ReceiverConfiguration receiver, string dbConnectionString)
    {
        receiver.Services.AddHostedService<InboxConsumerBackgroundService>();
        receiver.Services.AddHostedService<InboxOldMessagesDeleterBackgroundService>();
        receiver.Services.AddTransient<IInboxRepository>(_ => new InboxRepository(dbConnectionString));
        receiver.Services.AddTransient<IConsumingRunner, ConsumingRunner>();
        receiver.Services.AddTransient<IMissingOutboxTableCreator, MissingOutboxTableCreator>();
        receiver.Services.AddTransient<IOldProcessedMessagesDeleter, OldProcessedMessagesDeleter>();
    }
}