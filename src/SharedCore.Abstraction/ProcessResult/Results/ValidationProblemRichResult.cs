using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public sealed class ValidationProblemRichResult : RichResult
{
    private readonly IDictionary<string, string[]> _validationProblems;

    public ValidationProblemRichResult(IDictionary<string, string[]> validationProblems) : base(false)
    {
        _validationProblems = validationProblems;
    }

    public override void Evaluate(IResultsEvaluator evaluator)
    {
        evaluator.ValidationProblem(_validationProblems);
    }
}