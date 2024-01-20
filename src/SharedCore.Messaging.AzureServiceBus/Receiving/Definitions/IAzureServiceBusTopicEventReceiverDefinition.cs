using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using SharedCore.Messaging.AzureServiceBus.Receiving.Processors;
using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.AzureServiceBus.Receiving.Definitions;

public interface IAzureServiceBusTopicEventReceiverDefinition : IEventReceiverDefinition
{
    void LogReceivingError(ILogger<AzureServiceBus> logger, ProcessErrorEventArgs processErrorEventArgs);
    IRichServiceBusProcessor GetProcessor(ServiceBusClient client);
}
