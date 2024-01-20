using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedCore.Features.MediatR;

public static class SharedCoreMediatRExtensions
{
    public static void AddLoggingBehavior(this MediatRServiceConfiguration cfg)
    {
        cfg.AddOpenBehavior(typeof(OpenLoggingBehavior<,>));
    }

    public static void AddOptionalValidationBehavior(this MediatRServiceConfiguration cfg)
    {
        cfg.AddOpenBehavior(typeof(OpenValidatorBehavior<,>));
    }
    
    public static void AddRichMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddLoggingBehavior();
            cfg.AddOptionalValidationBehavior();
        });
    }
}