using System.Security.Claims;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.OrderService.Presentation.Http;

public static class HttpExtensions
{
    public static UserId? GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        return Guid.TryParse(userId, out var userGuid) ? new UserId(userGuid) : null; 
    }
}