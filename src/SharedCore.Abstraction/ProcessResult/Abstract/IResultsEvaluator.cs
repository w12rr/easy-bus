namespace SharedCore.Abstraction.ProcessResult.Abstract;

public interface IResultsEvaluator
{
    void Created<T>(T obj)
        where T : notnull;

    void NoContent();

    void Ok<T>(T obj, RichRange? range = default);
    void Ok();

    void NotFound(string title, string errorKey, params string[] errorDescriptions);

    void NotFound(string title, params (string errorKey, string[] errorDescriptions)[] errors);

    void Problem(IDictionary<string, string[]> toDictionary);
    void Conflict(string? message);
    void UnprocessableEntity(string? message);
    void ValidationProblem(IDictionary<string, string[]> validationProblems);
    void Forbidden();
}