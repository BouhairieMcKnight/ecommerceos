namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string requestCacheKey,
        string tag,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? timeSpan = null,
        CancellationToken cancellationToken = default);
    
    Task RemoveAsync(string tag, CancellationToken cancellationToken = default);
}