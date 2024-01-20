using EasyBus.Core.Definitions;
using EasyBus.Core.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyBus.Infrastructure.DependencyInjection;

public class ReceiverConfiguration
{
    private readonly IServiceCollection _services;

    public ReceiverConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public void AddDefinition(IEventReceiverDefinition definition) =>
        _services.AddScoped<IEventReceiverDefinition>(_ => definition);

    public void AddDefinition(Func<IServiceProvider, IEventReceiverDefinition> factory) => _services.AddScoped(factory);

    public void AddMessageReceiver<T>()
        where T : class, IMessageReceiver
    {
        _services.AddScoped<IMessageReceiver, T>();
    }

    public void AddAdditionalServices<TService, TImpl>() 
        where TImpl : class, TService 
        where TService : class
    {
        _services.TryAddScoped<TService, TImpl>();
    }

    // public void AddReceiverHostedServices<THostedService>() 
        // where THostedService : class, IHostedService
    // {
        // _services.AddHostedService<THostedService>();
    // }
}