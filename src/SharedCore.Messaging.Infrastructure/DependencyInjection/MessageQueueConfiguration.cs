using Microsoft.Extensions.DependencyInjection;
using SharedCore.Messaging.Core;
using SharedCore.Messaging.Core.Publishing;
using SharedCore.Messaging.Core.Receiving;
using IPublisher = SharedCore.Messaging.Core.Publishing.IPublisher;

namespace SharedCore.Messaging.Infrastructure.DependencyInjection;

public class MessageQueueConfiguration
{
    private readonly IServiceCollection _services;

    public MessageQueueConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public void AddPublisher(Action<PublisherConfiguration> config)
    {
        _services.AddScoped<IPublisher, Publisher>();
        var publisherConfig = new PublisherConfiguration(_services);
        config(publisherConfig);
    }

    public void AddReceiver(Action<ReceiverConfiguration> config)
    {
        _services.AddScoped<IReceiver, Receiver>();
        // _services.AddHostedService<ReceiverHostedService>();
        var receiverConfig = new ReceiverConfiguration(_services);
        // receiverConfig.AddMessageReceiver<SimpleMessageReceiver>();
        config(receiverConfig);
    }

    public void AddMessageQueueOptions<TOption>()
        where TOption : class
    {
        _services.AddOptions<TOption>();
    }

    public void PostConfigureMessageQueueOptions<T>(Action<T> configure)
        where T : class
    {
        _services.PostConfigure(configure);
    }

    public void AddMessageQueue(Func<IServiceProvider, IMessageQueue> messageQueueFactory)
    {
        _services.AddScoped(messageQueueFactory);
    }
}