using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace SharedCore.Features.FluentValidation;

public abstract class OptionsValidator<T> : AbstractValidator<T>, IValidateOptions<T>
    where T : class
{
    public ValidateOptionsResult Validate(string? name, T? options)
    {
        if (options is null) return ValidateOptionsResult.Fail(Consts.ErrorCodes.OptionCannotBeNull);

        var validationResult = base.Validate(options);

        return validationResult.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(
                $"Found configuration error: \n{GetFormattedMessage(validationResult)}");
    }

    private static string GetFormattedMessage(ValidationResult validationResult)
    {
        return string.Join(
            "\n\n",
            validationResult.Errors.Select(x =>
                $"Message: {x.ErrorMessage}\nError code: {x.ErrorCode}\nProperty: {x.PropertyName}"));
    }
}