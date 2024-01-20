using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedCore.Features.FluentValidation;
using SharedCore.Messaging.Core;
using SharedCore.Messaging.Core.Publishing;
using SharedCore.Messaging.Core.Receiving;
using IPublisher = SharedCore.Messaging.Core.Publishing.IPublisher;

namespace SharedCore.Messaging.Infrastructure.DependencyInjection;

public class MessageQueueConfiguration
{
    private readonly IServiceCollection _services;

    public MessageQueueConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public void AddPublisher(Action<PublisherConfiguration> config)
    {
        _services.AddScoped<IPublisher, Publisher>();
        var publisherConfig = new PublisherConfiguration(_services);
        config(publisherConfig);
    }

    public void AddReceiver(Action<ReceiverConfiguration> config)
    {
        _services.AddScoped<IReceiver, Receiver>();
        _services.AddHostedService<ReceiverHostedService>();
        var receiverConfig = new ReceiverConfiguration(_services);
        receiverConfig.AddMessageReceiver<SimpleMessageReceiver>();
        config(receiverConfig);
    }

    public void AddMessageQueueOptions<TOption, TValidator>()
        where TOption : class
        where TValidator : AbstractValidator<TOption>, IValidateOptions<TOption>
    {
        _services.AddValidatedOptions<TOption, TValidator>();
    }

    public void PostConfigureMessageQueueOptions<T>(Action<T> configure)
        where T : class
    {
        _services.PostConfigure(configure);
    }

    public void AddMessageQueue(Func<IServiceProvider, IMessageQueue> messageQueueFactory)
    {
        _services.AddScoped(messageQueueFactory);
    }
}