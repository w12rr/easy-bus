using Azure.Messaging.ServiceBus;
using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.AzureServiceBus.Publishing.Definitions;


public interface IAzureServiceBusEventPublishingDefinition<T> : IEventPublishingDefinition<T>
{
    ServiceBusSender  CreateSender(ServiceBusClient client);
    ServiceBusMessage CreateMessage(T @event);
}