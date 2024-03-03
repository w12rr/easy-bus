using Microsoft.Extensions.DependencyInjection;

namespace EasyBus.InMemory.Receivers;

public class InMemoryReceiverPostConfiguration<T>
{
    private readonly IServiceCollection _services;

    public InMemoryReceiverPostConfiguration(IServiceCollection services)
    {
        _services = services;
    }
    
    public InMemoryReceiverPostConfiguration<T> AddFuncHandler(Func<IServiceProvider, T, Task> onSuccess)
    {
        _services.AddScoped<IInMemoryMessageHandler<T>>(
            sp => new FuncInMemoryMessageHandler<T>(
                async @event => await onSuccess(sp, @event)));
        return this;
    }
}