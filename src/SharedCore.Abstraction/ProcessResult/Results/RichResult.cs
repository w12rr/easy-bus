using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public abstract class RichResult : IRichResult
{
    protected RichResult(bool succeeded)
    {
        Succeeded = succeeded;
    }

    public bool Succeeded { get; }
    public abstract void Evaluate(IResultsEvaluator evaluator);

    public static IRichResult NoContent() => new NoContentRichResult();
    public static IRichResult Forbidden() => new ForbiddenRichResult();

    public static IRichResult NotFound(string title, string errorKey, params string[] errorDescriptions) =>
        new NotFoundRichResult(title, errorKey, errorDescriptions);

    public static IRichResult NotFound(string title) =>
        new NotFoundRichResult(title, errorKey: string.Empty);

    public static IRichResult Created<T>(T obj)
        where T : notnull =>
        new CreatedRichResult<T>(obj);

    public static IRichResult Created(Guid id) => new CreatedRichResult<object>(new { Id = id });

    public static IRichResult Ok<T>(T obj, RichRange? range = default) => new OkRichResult<T>(obj, range);
    public static IRichResult Ok(Guid id) => new OkRichResult<object>(new { Id = id });
    public static IRichResult Ok() => new OkRichResult();
    public static IRichResult Conflict(string? message) => new ConflictRichResult(message);
    public static IRichResult UnprocessableEntity(string? message) => new UnprocessableEntityRichResult(message);

    public static IRichResult ValidationProblem(IDictionary<string, string[]> validationProblems) =>
        new ValidationProblemRichResult(validationProblems);
}

public abstract class RichResult<T> : RichResult, IRichResult<T>
{
    protected RichResult(T? result, bool succeeded) : base(succeeded)
    {
        if (succeeded && result is null)
            throw new Exception(
                $"When {nameof(succeeded)} is set to true the parameter {nameof(result)} must not be null");

        Result = result;
    }

    public T? Result { get; }
}