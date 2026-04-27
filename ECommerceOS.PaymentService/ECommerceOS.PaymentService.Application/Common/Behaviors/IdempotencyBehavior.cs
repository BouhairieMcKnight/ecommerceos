namespace ECommerceOS.PaymentService.Application.Common.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyService idempotencyService) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (await idempotencyService.RequestExistsAsync(request.IdempotentCommandId, cancellationToken))
        {
            return CreateConflictResult();
        }
        
        await idempotencyService.CreateRequestAsync(
            request.IdempotentCommandId, typeof(TRequest).Name, cancellationToken);
        
        var response = await next(cancellationToken);
        
        return response;
    }

    private static TResponse CreateConflictResult()
    {
        var error = Error.Conflict(
            "Request.Conflict",
            "A request with this idempotency key has already been processed.");

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(genericType)
                .GetMethod(nameof(Result<object>.Failure), [typeof(Error)])!;

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Unsupported response type for idempotency behavior: {typeof(TResponse).Name}");
    }
}
