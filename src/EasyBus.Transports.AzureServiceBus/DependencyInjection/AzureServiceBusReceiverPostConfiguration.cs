using Azure.Messaging.ServiceBus;
using EasyBus.Transports.AzureServiceBus.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyBus.Transports.AzureServiceBus.DependencyInjection;

public class AzureServiceBusReceiverPostConfiguration<T>
{
    public IServiceCollection Services;

    public AzureServiceBusReceiverPostConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public AzureServiceBusReceiverPostConfiguration<T> SetFuncHandler(
        Func<IServiceProvider, ProcessMessageEventArgs, T, Task> onSuccess,
        Func<IServiceProvider, ProcessErrorEventArgs, Task>? onError = default)
    {
        Services.AddScoped<IAzureServiceBusMessageHandler<T>>(sp =>
        {
            return new FuncAzureServiceBusMessageHandler<T>(
                sp.GetRequiredService<ILogger<FuncAzureServiceBusMessageHandler<T>>>(),
                async (args, @event) => await onSuccess(sp, args, @event),
                async args => await (onError?.Invoke(sp, args) ?? Task.CompletedTask));
        });
        return this;
    }

    public AzureServiceBusReceiverPostConfiguration<T> SetHandler<THandler>()
        where THandler : class, IAzureServiceBusMessageHandler<T>
    {
        Services.AddScoped<IAzureServiceBusMessageHandler<T>, THandler>();
        return this;
    }

    public AzureServiceBusReceiverPostConfiguration<T> SetFuncHandler(Func<IServiceProvider, T, Task<bool>> onSuccess,
        Func<IServiceProvider, ProcessErrorEventArgs, Task>? onError = default)
    {
        SetFuncHandler(
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