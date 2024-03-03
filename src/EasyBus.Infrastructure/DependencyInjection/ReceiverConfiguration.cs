using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.Infrastructure.DependencyInjection;

public class ReceiverConfiguration
{
    public IServiceCollection Services;

    public ReceiverConfiguration(IServiceCollection services)
    {
        Services = services;
    }
}