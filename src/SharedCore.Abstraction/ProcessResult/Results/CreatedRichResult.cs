using SharedCore.Abstraction.ProcessResult.Abstract;
using SharedCore.Abstraction.Services;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class CreatedRichResult<T> : RichResult<T>
    where T : notnull
{
    public CreatedRichResult(T obj) : base(obj, true)
    {
    }

    public override void Evaluate(IResultsEvaluator evaluator) => evaluator.Created(Result.AssertNull());
}