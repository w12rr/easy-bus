using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public class GenericErrorResult<T> : IRichResult<T>
{
    private readonly IRichResult _nonGenericResult;

    public GenericErrorResult(IRichResult nonGenericResult)
    {
        if (nonGenericResult.Succeeded)
        {
            throw new Exception(
                $"{nameof(GenericErrorResult<T>)} accepts only {nameof(nonGenericResult)} that not succeeded");
        }
        _nonGenericResult = nonGenericResult;
    }
    
    public void Evaluate(IResultsEvaluator evaluator)
    {
        _nonGenericResult.Evaluate(evaluator);
    }

    public bool Succeeded => _nonGenericResult.Succeeded;
    public T? Result => default;
}