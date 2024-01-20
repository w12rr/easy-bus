using Azure.Messaging.ServiceBus;

namespace SharedCore.Messaging.AzureServiceBus.Receiving.Processors;

public class RichServiceBusProcessor : IRichServiceBusProcessor
{
    private readonly ServiceBusProcessor _serviceBusProcessor;
    private Func<RichServiceBusMessage, Task>? _onMessage;

    public RichServiceBusProcessor(ServiceBusProcessor serviceBusProcessor)
    {
        _serviceBusProcessor = serviceBusProcessor;
    }

    public string Identifier => _serviceBusProcessor.Identifier;

    public void AttachMessageEvent(Func<RichServiceBusMessage, Task> onMessage)
    {
        _onMessage = onMessage;
        _serviceBusProcessor.ProcessMessageAsync += OnMessage;
    }

    public void AttachErrorEvent(Func<ProcessErrorEventArgs, Task> onError)
    {
        _serviceBusProcessor.ProcessErrorAsync += onError;
    }

    public async Task StopProcessing(CancellationToken cancellationToken)
    {
        await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
    }

    private async Task OnMessage(ProcessMessageEventArgs arg)
    {
        if (_onMessage is null)
            throw new NullReferenceException(
                $"{nameof(_onMessage)} is null but event was performed: ${arg.Message}, {arg.FullyQualifiedNamespace}");

        await _onMessage(new RichServiceBusMessage
        {
            Identifier = arg.Identifier,
            Message = arg.Message,
            CancellationToken = arg.CancellationToken
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceBusProcessor.DisposeAsync();
    }
}