using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Infrastructure.DependencyInjection;

public class PublisherConfiguration
{
    public IServiceCollection Services;

    public PublisherConfiguration(IServiceCollection services)
    {
        Services = services;
    }
}