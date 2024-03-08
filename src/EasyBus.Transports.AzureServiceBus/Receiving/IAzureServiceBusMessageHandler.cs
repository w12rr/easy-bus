using Azure.Messaging.ServiceBus;

namespace EasyBus.Transports.AzureServiceBus.Receiving;

public interface IAzureServiceBusMessageHandler<in T>
{
    Task MessageHandler(ProcessMessageEventArgs args, T @event);
    Task ErrorHandler(ProcessErrorEventArgs args);
}