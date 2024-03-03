using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
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
        string topicOrQueueName,
        Func<ServiceBusClientOptions>? serviceBusClientOptionsFactory = default,
        Func<TokenCredential>? tokenCredentialFactory = default,
        Action<T, ServiceBusMessage>? messageInterceptor = default,
        Func<T, ServiceBusMessage>? messageFactory = default)
    {
        configuration.Services.AddScoped<IInfrastructurePublisher<T>, AzureServiceBusInfrastructurePublisher<T>>(
            sp =>
            {
                var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
                return new AzureServiceBusInfrastructurePublisher<T>(
                    options,
                    topicOrQueueName,
                    serviceBusClientOptionsFactory ?? (() => new ServiceBusClientOptions
                    {
                        TransportType = ServiceBusTransportType.AmqpWebSockets
                    }),
                    tokenCredentialFactory ?? (() => new DefaultAzureCredential()),
                    messageInterceptor ?? ((_, _) => { }),
                    messageFactory ?? (arg => new ServiceBusMessage(JsonSerializer.Serialize(arg))));
            });
    }

    public static ReceiverConfiguration<T> AddAzureServiceBusTopicReceiver<T>(this ReceiverConfiguration configuration,
        string mqName,
        string topicName,
        string subscriptionName)
    {
        configuration.Services.AddScoped<IAzureServiceBusMessageHandler<T>, LoggerAzureServiceBusMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, AzureServiceBusTopicInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
            return new AzureServiceBusTopicInfrastructureReceiver<T>(options,
                sp.GetRequiredService<IAzureServiceBusMessageHandler<T>>(),
                topicName, subscriptionName);
        });
        return new ReceiverConfiguration<T>(configuration.Services);
    }

    public static ReceiverConfiguration<T> AddAzureServiceBusQueueReceiver<T>(this ReceiverConfiguration configuration,
        string mqName,
        string queueName)
    {
        configuration.Services.AddScoped<IAzureServiceBusMessageHandler<T>, LoggerAzureServiceBusMessageHandler<T>>();
        configuration.Services.AddScoped<IInfrastructureReceiver, AzureServiceBusQueueInfrastructureReceiver<T>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[mqName];
            return new AzureServiceBusQueueInfrastructureReceiver<T>(options,
                sp.GetRequiredService<IAzureServiceBusMessageHandler<T>>(),
                queueName);
        });
        return new ReceiverConfiguration<T>(configuration.Services);
    }
}