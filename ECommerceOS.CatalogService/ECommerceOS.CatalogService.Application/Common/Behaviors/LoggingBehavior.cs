namespace ECommerceOS.CatalogService.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting request {@RequestName}, {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTimeOffset.UtcNow);

        var result = await next(cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "Request failure {@RequestName}, {@Error}, {@DateTimeUtc}",
                typeof(TRequest).Name,
                result.Error,
                DateTimeOffset.UtcNow);
        }
        
        logger.LogInformation(
            "Completed request {@RequestName}, {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTimeOffset.UtcNow);
        
        return result;
    }
}