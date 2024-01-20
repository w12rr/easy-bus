using Azure.Messaging.ServiceBus;

namespace EasyBus.AzureServiceBus.Receiving;

public sealed class RichServiceBusMessage
{
    public required string Identifier { get; init; }
    public required ServiceBusReceivedMessage Message { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}