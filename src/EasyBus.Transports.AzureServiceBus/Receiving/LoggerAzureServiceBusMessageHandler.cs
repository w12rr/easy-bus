using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.AzureServiceBus.Receiving;

public class LoggerAzureServiceBusMessageHandler<T> : IAzureServiceBusMessageHandler<T>
{
    private readonly ILogger<LoggerAzureServiceBusMessageHandler<T>> _logger;

    public LoggerAzureServiceBusMessageHandler(ILogger<LoggerAzureServiceBusMessageHandler<T>> logger)
    {
        _logger = logger;
    }
    
    public async Task MessageHandler(ProcessMessageEventArgs args, T @event)
    {
        _logger.LogInformation("Received event: {MessageId} {CorrelationId} {PartitionKey} {EventName} {@EventData}", 
            args.Message.MessageId, args.Message.CorrelationId, args.Message.PartitionKey, typeof(T).Name, @event);
        await args.CompleteMessageAsync(args.Message);
    }

    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogInformation(args.Exception, "Handled error {Source} {EventName}", args.ErrorSource, typeof(T).Name);
        return Task.CompletedTask;
    }
}