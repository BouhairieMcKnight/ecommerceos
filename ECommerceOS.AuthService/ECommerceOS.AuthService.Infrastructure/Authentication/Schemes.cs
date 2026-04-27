using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace ECommerceOS.AuthService.Infrastructure.Authentication;

public static class Schemes
{
    public const string DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    public const string GoogleOidcScheme = GoogleDefaults.AuthenticationScheme;
    public const string ExternalCookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}