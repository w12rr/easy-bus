using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Infrastructure.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static void AddMessageQueue(this IServiceCollection services, Action<MessageQueueConfiguration> config)
    {
        var mqConfig = new MessageQueueConfiguration(services);
        config(mqConfig);
    }
}