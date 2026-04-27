namespace ECommerceOS.Shared.DTOs.Query;

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

public interface ICachedQuery
{
    string CacheKey { get; }
    string Tag { get; }
    TimeSpan CacheDuration { get; }
}