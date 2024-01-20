using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class UnprocessableEntityRichResult : RichResult
{
    private readonly string? _message;

    public UnprocessableEntityRichResult(string? message) : base(false)
    {
        _message = message;
    }

    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.UnprocessableEntity(_message);
}