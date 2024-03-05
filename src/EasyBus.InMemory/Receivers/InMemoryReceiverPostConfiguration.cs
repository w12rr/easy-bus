using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.InMemory.Receivers;

public class InMemoryReceiverPostConfiguration<T> 
{
    public readonly IServiceCollection Services;

    public InMemoryReceiverPostConfiguration(IServiceCollection services)
    {
        Services = services;
    }
    
    public InMemoryReceiverPostConfiguration<T> SetFuncHandler(Func<IServiceProvider, T, Task> onSuccess)
    {
        Services.AddScoped<IInMemoryMessageHandler<T>>(
            sp => new FuncInMemoryMessageHandler<T>(
                async @event => await onSuccess(sp, @event)));
        return this;
    }
}