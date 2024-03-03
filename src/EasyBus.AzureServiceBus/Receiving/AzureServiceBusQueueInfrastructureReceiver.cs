using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Options;
using EasyBus.Core.Helpers;
using EasyBus.Core.InfrastructureWrappers;

namespace EasyBus.AzureServiceBus.Receiving;

public class AzureServiceBusQueueInfrastructureReceiver<T> : IInfrastructureReceiver
{
    private readonly ServiceBusConnectionOptions _serviceBusConnectionOptions;
    private readonly IAzureServiceBusMessageHandler<T> _messageHandler;
    private readonly string _queueName;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;


    public AzureServiceBusQueueInfrastructureReceiver(ServiceBusConnectionOptions serviceBusConnectionOptions,
        IAzureServiceBusMessageHandler<T> messageHandler,
        string queueName)
    {
        _serviceBusConnectionOptions = serviceBusConnectionOptions;
        _messageHandler = messageHandler;
        _queueName = queueName;
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(_serviceBusConnectionOptions.ConnectionString, new DefaultAzureCredential());
        _processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var @event = JsonSerializer.Deserialize<T>(body).AssertNull();
        await _messageHandler.MessageHandler(args, @event);
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        await _messageHandler.ErrorHandler(args);
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor is not null)
        {
            _processor.ProcessMessageAsync -= MessageHandler;
            _processor.ProcessErrorAsync -= ErrorHandler;
            await _processor.DisposeAsync();
        }

        if (_client is not null)
            await _client.DisposeAsync();
    }
}