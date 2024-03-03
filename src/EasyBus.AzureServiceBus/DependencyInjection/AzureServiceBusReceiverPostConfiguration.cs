using Azure.Messaging.ServiceBus;
using EasyBus.AzureServiceBus.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyBus.AzureServiceBus.DependencyInjection;

public class AzureServiceBusReceiverPostConfiguration<T>
{
    private readonly IServiceCollection _services;

    public AzureServiceBusReceiverPostConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public AzureServiceBusReceiverPostConfiguration<T> AddFuncHandler(
        Func<IServiceProvider, ProcessMessageEventArgs, T, Task> onSuccess,
        Func<IServiceProvider, ProcessErrorEventArgs, Task>? onError = default)
    {
        _services.AddScoped<IAzureServiceBusMessageHandler<T>>(sp =>
        {
            return new FuncAzureServiceBusMessageHandler<T>(
                sp.GetRequiredService<ILogger<FuncAzureServiceBusMessageHandler<T>>>(),
                async (args, @event) => await onSuccess(sp, args, @event),
                async args => await (onError?.Invoke(sp, args) ?? Task.CompletedTask));
        });
        return this;
    }

    public AzureServiceBusReceiverPostConfiguration<T> AddFuncHandler(
        Func<IServiceProvider, T, Task<bool>> onSuccess,
        Func<IServiceProvider, ProcessErrorEventArgs, Task>? onError = default)
    {
        AddFuncHandler(
            async (sp, args, @event) =>
            {
                if (await onSuccess(sp, @event))
                {
                    await args.CompleteMessageAsync(args.Message);
                }
            },
            onError);
        return this;
    }
}