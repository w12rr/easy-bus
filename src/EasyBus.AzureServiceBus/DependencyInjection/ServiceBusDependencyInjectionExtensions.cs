using EasyBus.AzureServiceBus.Options;
using EasyBus.AzureServiceBus.Publishing.Definitions;
using EasyBus.AzureServiceBus.Receiving.Definitions;
using EasyBus.Core.Helpers;
using EasyBus.Core.Receiving;
using EasyBus.Infrastructure.DependencyInjection;
using EasyBus.Infrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasyBus.AzureServiceBus.DependencyInjection;

public static class ServiceBusDependencyInjectionExtensions
{
    public static void AddAzureServiceBus(this MessageQueueConfiguration mqConfiguration, string name,
        object configuration)
    {
        mqConfiguration.AddMessageQueueOptions<AzureServiceBusOptions>();
        mqConfiguration.PostConfigureMessageQueueOptions<AzureServiceBusOptions>(option =>
        {
            // option.Connections.Add(name, configuration.Get<ServiceBusConnectionOptions>().AssertNull());
        });
        mqConfiguration.AddMessageQueue(sp => new AzureServiceBus(
            sp.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value.Connections[name],
            name,
            // sp.GetRequiredService<ILogger<AzureServiceBus>>(),
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