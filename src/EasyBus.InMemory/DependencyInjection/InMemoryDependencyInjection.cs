using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.InMemory.Channels;
using EasyBus.InMemory.Publishers;
using EasyBus.InMemory.Receivers;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.InMemory.DependencyInjection;

public static class InMemoryDependencyInjection
{
    public static void AddInMemoryMq(this MessageQueueConfiguration mqConfig)
    {
        mqConfig.Services.AddSingleton<IChannelsStore, ChannelsStore>();
    }

    public static void AddInMemoryEventPublisher<T>(this PublisherConfiguration config)
    {
        config.Services.AddScoped<IInfrastructurePublisher<T>, ChannelPublisher<T>>();
    }

    public static InMemoryReceiverPostConfiguration<T> AddInMemoryReceiver<T>(this ReceiverConfiguration configuration)
    {
        configuration.Services.AddScoped<IInMemoryMessageHandler<T>, LoggerInMemoryMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, ChannelReceiver<T>>(
            sp =>
                new ChannelReceiver<T>(
                    sp.GetRequiredService<IChannelsStore>(),
                    sp.GetRequiredService<IInMemoryMessageHandler<T>>()));
        return new InMemoryReceiverPostConfiguration<T>(configuration.Services);
    }
}