using ZiggyCreatures.Caching.Fusion;

namespace ECommerceOS.AuthService.Infrastructure.Caching;

public class CacheService(IFusionCache cache) : ICacheService
{
    public async Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        string tag,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? timeSpan = null,
        CancellationToken cancellationToken = default)
    {
        var result = await cache.GetOrSetAsync(
            cacheKey,
            async ct => await factory(ct),
            tags: [tag],
            token: cancellationToken
        );
        
        return result;
    }

    public async Task RemoveAsync(string tag, CancellationToken cancellationToken = default)
    {
        await cache.RemoveByTagAsync(tag, token: cancellationToken);
    }
}