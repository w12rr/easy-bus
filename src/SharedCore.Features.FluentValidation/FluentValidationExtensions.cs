using System.Reflection;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedCore.Abstraction.Services;

namespace SharedCore.Features.FluentValidation;

public static class FluentValidationExtensions
{
    public static void AddValidatedOptions<TOptions, TValidator>(this IServiceCollection services, IConfigurationSection section)
        where TOptions: class
        where TValidator: AbstractValidator<TOptions>, IValidateOptions<TOptions>
    {
        services.AddSingleton<IValidateOptions<TOptions>, TValidator>();
        services.AddOptions<TOptions>().Bind(section).ValidateOnStart();
    }
    
    public static void AddValidatedOptions<TOptions, TValidator>(this IServiceCollection services)
        where TOptions: class
        where TValidator: AbstractValidator<TOptions>, IValidateOptions<TOptions>
    {
        services.AddSingleton<IValidateOptions<TOptions>, TValidator>();
        services.AddOptions<TOptions>().ValidateOnStart();
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValidLink<T>(this IRuleBuilderInitial<T, string> builder)
    {
        var anyWhiteSpaceRegex = new Regex(@"\s");
        return builder.NotEmpty()
            .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
            .WithErrorCode(Consts.ErrorCodes.UriIsNotWellFormatted)
            .Must(x => !anyWhiteSpaceRegex.IsMatch(x))
            .WithErrorCode(Consts.ErrorCodes.UriIsNotWellFormatted)
            .Must(x => !x.EndsWith('/'))
            .WithErrorCode(Consts.ErrorCodes.UriCannotEndWithSlash);
    }
    
    public static IRuleBuilderOptions<T, string> MustBeDomainName<T>(this IRuleBuilder<T, string> builder)
    {
        var domainName = new Regex(@"^(([a-z0-9]+(-|_|[a-z0-9])*)\.)+[a-z0-9]+$");
        return builder.NotEmpty()
            .Must(x => !domainName.IsMatch(x))
            .WithErrorCode(Consts.ErrorCodes.MustBeDomainName);
    }

    public static void AddRichValidatorsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        var validators = assemblies.SelectMany(x => x.GetTypes())
            .Where(x => x.IsImplementationOfRichValidator())
            .Select(x => (implementation: x, abstraction: x.GetRichValidatorTypeDefinition()));

        foreach (var validator in validators)
        {
            services.AddScoped(validator.abstraction, validator.implementation);
        }
    }

    private static bool IsImplementationOfRichValidator(this Type t) => 
        t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRichValidator<>));

    private static Type GetRichValidatorTypeDefinition(this Type t) => 
        t.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRichValidator<>));
}