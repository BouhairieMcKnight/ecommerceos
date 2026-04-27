namespace ECommerceOS.OrderService.Application.Common.Behaviors;

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
        if (await idempotencyService.RequestExistsAsync(request.IdempotentCommandId))
        {
            return default;
        }
        
        await idempotencyService.CreateRequestAsync(request.IdempotentCommandId, typeof(TRequest).Name);
        
        var response = await next(cancellationToken);
        
        return response;
    }
}