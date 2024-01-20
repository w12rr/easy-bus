using Microsoft.Extensions.DependencyInjection;
using SharedCore.Messaging.Core.Definitions;

namespace SharedCore.Messaging.Infrastructure.DependencyInjection;

public class PublisherConfiguration
{
    private readonly IServiceCollection _services;

    public PublisherConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public void AddDefinition(IEventPublishingDefinition definition) =>
        _services.AddScoped<IEventPublishingDefinition>(_ => definition);

    public void AddDefinition(Func<IServiceProvider, IEventPublishingDefinition> factory) =>
        _services.AddScoped(factory);
}