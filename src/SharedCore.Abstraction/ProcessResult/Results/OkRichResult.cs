using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class OkRichResult<T> : RichResult<T>
{
    private readonly RichRange? _range;

    public OkRichResult(T obj, RichRange? range = default) : base(obj, true)
    {
        _range = range;
    }

    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.Ok(Result, _range);
}

public sealed class OkRichResult : RichResult
{
    public OkRichResult() : base(true)
    {
    }

    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.Ok();
}