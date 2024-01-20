using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Abstraction.ProcessResult.Results;

public static class RichResultExtensions
{
    public static IRichResult<T> ToGenericError<T>(this IRichResult nonGenericResult)
    {
        return new GenericErrorResult<T>(nonGenericResult);
    }
}