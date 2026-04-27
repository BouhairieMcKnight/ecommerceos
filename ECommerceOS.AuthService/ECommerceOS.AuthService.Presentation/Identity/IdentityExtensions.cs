using ECommerceOS.AuthService.Api.Identity.Delete;
using ECommerceOS.AuthService.Api.Identity.Login;
using ECommerceOS.AuthService.Api.Identity.Logout;
using ECommerceOS.AuthService.Api.Identity.OAuth;
using ECommerceOS.AuthService.Api.Identity.Refresh;
using ECommerceOS.AuthService.Api.Identity.Register;
using ECommerceOS.AuthService.Api.Identity.VerifyEmail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ECommerceOS.AuthService.Api.Identity;

public static class IdentityExtensions
{
    private const string RouteGroupPrefix = "/auth";

    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroupPrefix);

        group.MapDeleteEndpoint();
        group.MapVerificationEndpoint();
        group.MapLogoutEndpoint();
        group.MapRegisterEndpoint();
        group.MapLoginEndpoint();
        group.MapOauthEndpoint();
        group.MapRefreshEndpoint();


        return group;
    }
}