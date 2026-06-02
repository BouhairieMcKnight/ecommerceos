using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ECommerceOS.PaymentService.Infrastructure.Auth;

public static class Schemes
{
    public const string DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    public const string ExternalCookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}