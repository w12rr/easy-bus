using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EasyBus.AzureServiceBus.Receiving;

public class FuncAzureServiceBusMessageHandler<T> : IAzureServiceBusMessageHandler<T>
{
    private readonly ILogger<FuncAzureServiceBusMessageHandler<T>> _logger;
    private readonly Func<ProcessMessageEventArgs, T, Task> _onSuccess;
    private readonly Func<ProcessErrorEventArgs, Task>? _onError;

    public FuncAzureServiceBusMessageHandler(
        ILogger<FuncAzureServiceBusMessageHandler<T>> logger,
        Func<ProcessMessageEventArgs, T, Task> onSuccess,
        Func<ProcessErrorEventArgs, Task>? onError = default)
    {
        _logger = logger;
        _onSuccess = onSuccess;
        _onError = onError;
    }
    
    public async Task MessageHandler(ProcessMessageEventArgs args, T @event)
    {
        await _onSuccess(args, @event);
    }

    public async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        if (_onError is not null)
        {
            await _onError(args);
        }
        else
        {
            _logger.LogInformation(args.Exception, "Handled error {Source} {EventName}", args.ErrorSource,
                typeof(T).Name);
        }
    }
}