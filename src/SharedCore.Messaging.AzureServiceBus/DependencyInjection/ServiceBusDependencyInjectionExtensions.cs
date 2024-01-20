using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCore.Abstraction.Services;
using SharedCore.Messaging.AzureServiceBus.Options;
using SharedCore.Messaging.AzureServiceBus.Publishing.Definitions;
using SharedCore.Messaging.AzureServiceBus.Receiving.Definitions;
using SharedCore.Messaging.Core.Receiving;
using SharedCore.Messaging.Infrastructure.DependencyInjection;
using SharedCore.Messaging.Infrastructure.Tools;

namespace SharedCore.Messaging.AzureServiceBus.DependencyInjection;

public static class ServiceBusDependencyInjectionExtensions
{
    public static void AddAzureServiceBus(this MessageQueueConfiguration mqConfiguration, string name,
        IConfigurationSection configuration)
    {
        mqConfiguration.AddMessageQueueOptions<AzureServiceBusOptions, AzureServiceBusOptionsValidator>();
        mqConfiguration.PostConfigureMessageQueueOptions<AzureServiceBusOptions>(option =>
        {
            option.Connections.Add(name, configuration.Get<ServiceBusConnectionOptions>().AssertNull());
        });
        mqConfiguration.AddMessageQueue(sp => new AzureServiceBus(
            sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[name],
            name,
            sp.GetRequiredService<ILogger<AzureServiceBus>>(),
            sp.GetRequiredService<IMessageReceiver>()));
    }

    public static void AddAzureServiceBusEventPublisher<T>(this PublisherConfiguration configuration,
        string messageQueueName,
        Func<T, string>? correlationIdFactory = default)
    {
        configuration.AddDefinition(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[messageQueueName];
            var topicName = TopicNames.NormalizeForType<T>();
            return new AzureServiceBusEventPublishingDefinition<T>(
                messageQueueName,
                Variables.TopicTemplate(options.Prefix, topicName),
                correlationIdFactory);
        });
    }

    public static void AddAzureServiceBusEventReceiver<T>(this ReceiverConfiguration configuration,
        string messageQueueName,
        bool enableSessions = false)
    {
        configuration.AddDefinition(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[messageQueueName];
            var topicName = TopicNames.NormalizeForType<T>();
            return new AzureServiceBusTopicEventReceiverDefinition<T>(
                messageQueueName,
                Variables.TopicTemplate(options.Prefix, topicName),
                Variables.SubscriptionTemplate(options.Prefix, topicName, options.SubscriptionSuffix.AssertNull()),
                enableSessions);
        });
    }
}