using MediatR;
using Microsoft.Extensions.Logging;
using SharedCore.Abstraction.ProcessResult.Abstract;

namespace SharedCore.Features.MediatR;

public class OpenLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<OpenLoggingBehavior<TRequest, TResponse>> _logger;

    public OpenLoggingBehavior(ILogger<OpenLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (response is IRichResult { Succeeded: false })
        {
            _logger.LogError(
                "The request of {RequestType} containing {@RequestedData} resulted in {@ResponseData}",
                typeof(TRequest).FullName,
                request,
                response);
        }

        return response;
    }
}