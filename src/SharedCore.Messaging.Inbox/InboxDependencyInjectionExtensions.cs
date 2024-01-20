using SharedCore.Messaging.Inbox.Consumer;
using SharedCore.Messaging.Inbox.HostedService;
using SharedCore.Messaging.Inbox.Persistence;
using SharedCore.Messaging.Infrastructure.DependencyInjection;

namespace SharedCore.Messaging.Inbox;

public static class InboxDependencyInjectionExtensions
{
    public static void AddInboxStore<TDatabaseContext>(this ReceiverConfiguration configuration)
        where TDatabaseContext : class, IInboxDbContext
    {
        configuration.AddMessageReceiver<InboxMessageReceiver>();
        configuration.AddAdditionalServices<IInboxDbContext, TDatabaseContext>();
        configuration.AddAdditionalServices<IInboxRepository, InboxRepository>();
    }
    
    public static void AddInboxMessageConsumer<TDatabaseContext>(this ReceiverConfiguration configuration)
        where TDatabaseContext : class, IInboxDbContext
    {
        configuration.AddAdditionalServices<IInboxMessageConsumer, InboxMessageConsumer>();
        configuration.AddAdditionalServices<IInboxDbContext, TDatabaseContext>();
        configuration.AddAdditionalServices<IInboxRepository, InboxRepository>();
        configuration.AddReceiverHostedServices<InboxHostedService>();
    }
}