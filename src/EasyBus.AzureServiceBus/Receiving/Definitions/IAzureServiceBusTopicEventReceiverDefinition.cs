using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Receiving.Processors;
using EasyBus.Core.Definitions;

namespace EasyBus.AzureServiceBus.Receiving.Definitions;

public interface IAzureServiceBusTopicEventReceiverDefinition : IEventReceiverDefinition
{
    void LogReceivingError(ProcessErrorEventArgs processErrorEventArgs);
    IRichServiceBusProcessor GetProcessor(ServiceBusClient client);
}
