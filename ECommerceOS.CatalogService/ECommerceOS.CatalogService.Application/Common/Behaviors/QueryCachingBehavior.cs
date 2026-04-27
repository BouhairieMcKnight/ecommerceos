using ECommerceOS.CatalogService.Application.Common.Interfaces;

namespace ECommerceOS.CatalogService.Application.Common.Behaviors;

public class QueryCachingBehavior<TRequest, TResponse>(ICacheService cacheService) 
    : IPipelineBehavior<TRequest, TResponse>
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
            async (ct) => await next(ct),
            request.CacheDuration,
            cancellationToken);
    }
}