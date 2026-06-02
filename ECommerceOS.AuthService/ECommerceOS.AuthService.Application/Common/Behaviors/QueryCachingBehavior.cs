using Microsoft.AspNetCore.Identity.Data;

namespace ECommerceOS.AuthService.Application.Common.Behaviors;

public sealed class QueryCachingBehavior<TRequest, TResponse>(ICacheService cacheService) : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await cacheService.GetOrCreateAsync(
            request.CacheKey, 
            request.Tag,
            ct => next(ct),
            request.CacheDuration,
            cancellationToken
        );
    }
}