using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class NoContentRichResult : RichResult
{
    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.NoContent();

    public NoContentRichResult() : base(true)
    {
    }
}

public sealed class ForbiddenRichResult : RichResult
{
    public ForbiddenRichResult() : base(false)
    {
    }

    public override void Evaluate(IResultsEvaluator evaluator)
    {
        evaluator.Forbidden();
    }
}