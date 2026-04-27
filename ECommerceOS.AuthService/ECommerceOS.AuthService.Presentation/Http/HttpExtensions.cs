namespace ECommerceOS.AuthService.Api.Http;

public static class HttpExtensions
{
    private const string RefreshTokenCookieName = "ecommerceos_refreshToken";
    
    extension(HttpContext httpContext)
    {
        public UserId? GetUserId()
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? httpContext.User.FindFirstValue("sub")
                         ?? string.Empty;
            return Guid.TryParse(userId, out var userGuid) ? new UserId(userGuid) : null; 
        }

        public string? GetRefreshToken()
        {
            return httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken)
                ? refreshToken
                : null;
        }

        public void SetRefreshToken(string refreshToken)
        {
            httpContext.Response.Cookies.Append(
                RefreshTokenCookieName,
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(7),
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                }
            );
        }

        public void ExpireRefreshTokenCookie()
        {
            httpContext.Response.Cookies.Append(
                RefreshTokenCookieName,
                string.Empty,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.MinValue,
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                }
            );
        }
    }
}
