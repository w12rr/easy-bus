using FluentValidation;
using SharedCore.Abstraction.ProcessResult.Abstract;
using SharedCore.Abstraction.ProcessResult.Results;
using SharedCore.Abstraction.Services;

namespace SharedCore.Features.FluentValidation;

public abstract class RichValidatorBase<T> : AbstractValidator<T>, IRichValidator<T>
{
    public async Task<IRichResult> RichValidateAsync(T validatable, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(validatable, cancellationToken);

        return validationResult.IsValid
            ? new OkRichResult()
            : new ValidationProblemRichResult(validationResult.ToDictionary());
    }
}