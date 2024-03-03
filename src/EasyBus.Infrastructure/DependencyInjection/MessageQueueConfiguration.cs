using EasyBus.Core.Publishing;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Infrastructure.DependencyInjection;

public class MessageQueueConfiguration
{
    public IServiceCollection Services { get; }

    public MessageQueueConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public void AddPublisher(Action<PublisherConfiguration> config)
    {
        Services.AddScoped<IPublisher, Publisher>();
        var publisherConfig = new PublisherConfiguration(Services);
        config(publisherConfig);
    }

    public void AddReceiver(Action<ReceiverConfiguration> config)
    {
        Services.AddHostedService<ReceiverBackgroundService>();
        var receiverConfig = new ReceiverConfiguration(Services);
        config(receiverConfig);
    }
}