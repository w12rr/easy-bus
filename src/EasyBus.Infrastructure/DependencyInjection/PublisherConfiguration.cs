using EasyBus.Core.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Infrastructure.DependencyInjection;

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