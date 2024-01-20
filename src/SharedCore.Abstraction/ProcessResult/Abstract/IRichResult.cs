namespace SharedCore.Abstraction.ProcessResult.Abstract;

public interface IRichResult
{
    void Evaluate(IResultsEvaluator evaluator);
    bool Succeeded { get; }
}

public interface IRichResult<out T> : IRichResult
{
    T? Result { get; }
}