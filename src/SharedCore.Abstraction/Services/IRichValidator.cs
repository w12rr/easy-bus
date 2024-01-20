using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.Services;

public interface IRichValidator<in TValidatable>
{
    Task<IRichResult> RichValidateAsync(TValidatable validatable, CancellationToken cancellationToken);
}