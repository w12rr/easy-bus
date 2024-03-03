using EasyBus.AzureServiceBus.Options;
using EasyBus.AzureServiceBus.Publishing;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasyBus.AzureServiceBus.DependencyInjection;

public static class ServiceBusDependencyInjectionExtensions
{
    public static void AddAzureServiceBus(this MessageQueueConfiguration mqConfiguration, string name,
        Action<ServiceBusConnectionOptions> asbOptionsConfig)
    {
        mqConfiguration.Services.AddOptions<AzureServiceBusOptions>();
        mqConfiguration.Services.PostConfigure<AzureServiceBusOptions>(opt =>
        {
            var asbOpt = new ServiceBusConnectionOptions();
            asbOptionsConfig(asbOpt);
            opt.Connections.Add(name, asbOpt);
        });
    }

    public static void AddAzureServiceBusEventPublisher<T>(this PublisherConfiguration configuration,
        string mqName,
        string topicOrQueueName)
    {
        configuration.Services.AddScoped<IInfrastructurePublisher<T>, AzureServiceBusInfrastructurePublisher<T>>(
            sp =>
            {
                var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
                return new AzureServiceBusInfrastructurePublisher<T>(options, topicOrQueueName);
            });
    }

    public static void AddAzureServiceBusTopicReceiver<T, TMessageHandler>(this ReceiverConfiguration configuration,
        string mqName,
        string topicName,
        string subscriptionName)
        where TMessageHandler : class, IAzureServiceBusMessageHandler<T>
    {
        configuration.Services.AddScoped<IAzureServiceBusMessageHandler<T>, TMessageHandler>();
        configuration.Services.AddScoped<IInfrastructureReceiver, AzureServiceBusTopicInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
            return new AzureServiceBusTopicInfrastructureReceiver<T>(options, sp.GetRequiredService<TMessageHandler>(),
                topicName, subscriptionName);
        });
    }

    public static void AddAzureServiceBusQueueReceiver<T, TMessageHandler>(this ReceiverConfiguration configuration,
        string mqName,
        string queueName)
        where TMessageHandler : class, IAzureServiceBusMessageHandler<T>
    {
        configuration.Services.AddScoped<IAzureServiceBusMessageHandler<T>, TMessageHandler>();
        configuration.Services.AddScoped<IInfrastructureReceiver, AzureServiceBusQueueInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
            return new AzureServiceBusQueueInfrastructureReceiver<T>(options, sp.GetRequiredService<TMessageHandler>(),
                queueName);
        });
    }
}