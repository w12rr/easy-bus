using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class NotFoundRichResult : RichResult
{
    private readonly string _title;
    private readonly string _errorKey;
    private readonly string[] _errorDescriptions;

    public NotFoundRichResult(string title, string errorKey, params string[] errorDescriptions) : base(false)
    {
        _title = title;
        _errorKey = errorKey;
        _errorDescriptions = errorDescriptions;
    }

    public override void Evaluate(IResultsEvaluator evaluator) =>
        evaluator.NotFound(_title, _errorKey, _errorDescriptions);
}