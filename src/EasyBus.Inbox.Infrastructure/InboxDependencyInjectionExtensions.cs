using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Inbox.Infrastructure;

public static class InboxDependencyInjectionExtensions
{
    public static void AddInboxMessageConsumer(this ReceiverConfiguration receiver)
    {
        receiver.Services.AddHostedService<InboxConsumerBackgroundService>();
    }
}