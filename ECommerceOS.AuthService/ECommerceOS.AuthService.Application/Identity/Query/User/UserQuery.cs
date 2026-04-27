using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.AuthService.Application.Identity.Query.User;

public record UserQuery(UserId UserId) : IQuery<UserQueryResponse>, ICachedQuery
{
    public string Tag => nameof(User);
    public string CacheKey => UserId.Value.ToString("D");
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(10);
}

public record UserQueryResponse(string Email, string Username);
