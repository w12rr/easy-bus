using Azure.Messaging.ServiceBus;
using SharedCore.Messaging.AzureServiceBus.Receiving.Processors;
using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.AzureServiceBus.Receiving.Definitions;

public interface IAzureServiceBusTopicEventReceiverDefinition : IEventReceiverDefinition
{
    void LogReceivingError(ProcessErrorEventArgs processErrorEventArgs);
    IRichServiceBusProcessor GetProcessor(ServiceBusClient client);
}
