using MediatR;
using Microsoft.Extensions.Logging;
using SharedCore.Abstraction.ProcessResult.Abstract;
using SharedCore.Abstraction.Services;

namespace SharedCore.Features.MediatR;

public class OpenValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IRichValidator<TRequest>> _richValidators;
    private readonly ILogger<OpenValidatorBehavior<TRequest, TResponse>> _logger;

    public OpenValidatorBehavior(IEnumerable<IRichValidator<TRequest>> richValidators,
        ILogger<OpenValidatorBehavior<TRequest, TResponse>> logger)
    {
        _richValidators = richValidators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validator = _richValidators.SingleOrDefault();
        if (validator is null) return await next();

        var validationResult = await validator.RichValidateAsync(request, cancellationToken);
        if (validationResult.Succeeded) return await next();

        try
        {
            return (TResponse)validationResult;
        }
        catch (InvalidCastException e)
        {
            throw new Exception(
                $"To use validator in pipeline behavior, handler return type must be {nameof(IRichResult)}, but was {typeof(TResponse).Name}",
                e);
        }
    }
}