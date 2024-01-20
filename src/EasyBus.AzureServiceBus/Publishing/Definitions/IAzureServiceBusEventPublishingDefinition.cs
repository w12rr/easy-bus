using Azure.Messaging.ServiceBus;
using EasyBus.Core.Definitions;

namespace EasyBus.AzureServiceBus.Publishing.Definitions;


public interface IAzureServiceBusEventPublishingDefinition<T> : IEventPublishingDefinition<T>
{
    ServiceBusSender  CreateSender(ServiceBusClient client);
    ServiceBusMessage CreateMessage(T @event);
}