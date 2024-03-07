using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Options;
using EasyBus.Core.Helpers;
using EasyBus.Core.InfrastructureWrappers;

namespace EasyBus.AzureServiceBus.Receiving;

public class AzureServiceBusTopicInfrastructureReceiver<T> : IInfrastructureReceiver, IAsyncDisposable
{
    private readonly ServiceBusConnectionOptions _serviceBusConnectionOptions;
    private readonly IAzureServiceBusMessageHandler<T> _messageHandler;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;


    public AzureServiceBusTopicInfrastructureReceiver(ServiceBusConnectionOptions serviceBusConnectionOptions,
        IAzureServiceBusMessageHandler<T> messageHandler,
        string topicName,
        string subscriptionName)
    {
        _serviceBusConnectionOptions = serviceBusConnectionOptions;
        _messageHandler = messageHandler;
        _topicName = topicName;
        _subscriptionName = subscriptionName;
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(_serviceBusConnectionOptions.ConnectionString, new DefaultAzureCredential());
        _processor = _client.CreateProcessor(_topicName, _subscriptionName, new ServiceBusProcessorOptions());
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