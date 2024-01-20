using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class ConflictRichResult : RichResult
{
    private readonly string? _message;

    public ConflictRichResult(string? message) : base(false)
    {
        _message = message;
    }

    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.Conflict(_message);
}