using Azure.Core;
using Azure.Messaging.ServiceBus;
using EasyBus.Core.InfrastructureWrappers;
using EasyBus.Transports.AzureServiceBus.Options;

namespace EasyBus.Transports.AzureServiceBus.Publishing;

public class AzureServiceBusInfrastructurePublisher<T> : IInfrastructurePublisher<T>
{
    private readonly ServiceBusConnectionOptions _serviceBusConnectionOptions;
    private readonly string _topicOrQueueName;
    private readonly Func<ServiceBusClientOptions> _serviceBusClientOptionsFactory;
    private readonly Func<TokenCredential> _tokenCredentialFactory;
    private readonly Action<T, ServiceBusMessage> _messageInterceptor;
    private readonly Func<T, ServiceBusMessage> _messageFactory;

    public AzureServiceBusInfrastructurePublisher(ServiceBusConnectionOptions serviceBusConnectionOptions,
        string topicOrQueueName,
        Func<ServiceBusClientOptions> serviceBusClientOptionsFactory,
        Func<TokenCredential> tokenCredentialFactory,
        Action<T, ServiceBusMessage> messageInterceptor,
        Func<T, ServiceBusMessage> messageFactory)
    {
        _serviceBusConnectionOptions = serviceBusConnectionOptions;
        _topicOrQueueName = topicOrQueueName;
        _serviceBusClientOptionsFactory = serviceBusClientOptionsFactory;
        _tokenCredentialFactory = tokenCredentialFactory;
        _messageInterceptor = messageInterceptor;
        _messageFactory = messageFactory;
    }

    public async Task Publish(T @event, CancellationToken cancellationToken)
    {
        var client = new ServiceBusClient(
            _serviceBusConnectionOptions.ConnectionString,
            _tokenCredentialFactory(),
            _serviceBusClientOptionsFactory());
        var sender = client.CreateSender(_topicOrQueueName);

        var message = _messageFactory(@event);
        _messageInterceptor(@event, message);

        await sender.SendMessageAsync(message, cancellationToken);
    }

    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        await Publish((T)@event, cancellationToken);
    }
}