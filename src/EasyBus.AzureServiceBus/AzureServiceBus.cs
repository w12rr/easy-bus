using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Options;
using EasyBus.AzureServiceBus.Publishing.Definitions;
using EasyBus.AzureServiceBus.Receiving;
using EasyBus.AzureServiceBus.Receiving.Definitions;
using EasyBus.AzureServiceBus.Receiving.Processors;
using EasyBus.Core;
using EasyBus.Core.Definitions;
using EasyBus.Core.Receiving;

namespace EasyBus.AzureServiceBus;

public class AzureServiceBus : IMessageQueue, IAsyncDisposable
{
    private readonly IMessageReceiver _messageReceiver;
    private readonly ServiceBusClient _client;
    private readonly SortedDictionary<string, IRichServiceBusProcessor> _serviceBusProcessors = new();

    private readonly SortedDictionary<string, IAzureServiceBusTopicEventReceiverDefinition>
        _receiversDefinitionsByProcessorId = new();

    public AzureServiceBus(ServiceBusConnectionOptions connectionOptions, string name,
        IMessageReceiver messageReceiver)
    {
        _messageReceiver = messageReceiver;
        _client = new ServiceBusClient(connectionOptions.ConnectionString);
        Name = name;
    }

    public string Name { get; }

    public async Task Publish<T>(IEventPublishingDefinition<T> definition, T @event,
        CancellationToken cancellationToken)
    {
        if (definition is not IAzureServiceBusEventPublishingDefinition<T> azureDefinition)
        {
            throw new ArgumentException(
                $"Argument {nameof(definition)} should be of type ${nameof(IAzureServiceBusEventPublishingDefinition<T>)}");
        }

        await using var sender = azureDefinition.CreateSender(_client);
        var message = azureDefinition.CreateMessage(@event);

        await Send(sender, message, cancellationToken);
    }

    public Task StartReceiving(IEventReceiverDefinition definition, CancellationToken cancellationToken)
    {
        if (definition is not IAzureServiceBusTopicEventReceiverDefinition azureDefinition)
        {
            throw new ArgumentException(
                $"Argument {nameof(definition)} should be of type ${nameof(IAzureServiceBusTopicEventReceiverDefinition)}");
        }

        var processor = azureDefinition.GetProcessor(_client);
        _serviceBusProcessors.Add(processor.Identifier, processor);
        _receiversDefinitionsByProcessorId.Add(processor.Identifier, azureDefinition);
        processor.AttachErrorEvent(ProcessError);
        processor.AttachMessageEvent(ProcessMessage);
        return Task.CompletedTask;
    }

    private Task ProcessError(ProcessErrorEventArgs arg)
    {
        var definition = _receiversDefinitionsByProcessorId[arg.Identifier];
        // definition.LogReceivingError(_logger, arg);
        return Task.CompletedTask;
    }

    private async Task ProcessMessage(RichServiceBusMessage arg)
    {
        var definition = _receiversDefinitionsByProcessorId[arg.Identifier];
        await _messageReceiver.Receive(
            arg.Message.Body.ToString(),
            arg.Message.CorrelationId,
            arg.Message.MessageId,
            definition,
            arg.CancellationToken);
    }

    public async Task StopReceiving(CancellationToken cancellationToken)
    {
        var keys = _serviceBusProcessors.Keys.ToList();
        foreach (var identifier in keys)
        {
            var processor = _serviceBusProcessors[identifier];
            await processor.StopProcessing(cancellationToken);
            _receiversDefinitionsByProcessorId.Remove(identifier);
        }
    }

    private static async Task Send(ServiceBusSender sender, ServiceBusMessage message,
        CancellationToken cancellationToken)
    {
        await sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        var keys = _serviceBusProcessors.Keys.ToList();
        foreach (var identifier in keys)
        {
            var processor = _serviceBusProcessors[identifier];
            await processor.DisposeAsync();
            _receiversDefinitionsByProcessorId.Remove(identifier);
            await _client.DisposeAsync();
        }
    }
}