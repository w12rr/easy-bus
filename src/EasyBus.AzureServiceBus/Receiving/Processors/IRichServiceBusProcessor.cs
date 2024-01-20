using Azure.Messaging.ServiceBus;

namespace EasyBus.AzureServiceBus.Receiving.Processors;

public interface IRichServiceBusProcessor : IAsyncDisposable
{
    string Identifier { get; }
    void AttachMessageEvent(Func<RichServiceBusMessage, Task> onMessage);
    void AttachErrorEvent(Func<ProcessErrorEventArgs, Task> onError);

    Task StopProcessing(CancellationToken cancellationToken);
}