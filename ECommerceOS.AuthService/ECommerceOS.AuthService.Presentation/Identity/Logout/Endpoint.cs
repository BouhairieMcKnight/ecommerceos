using ECommerceOS.AuthService.Api.Http;

namespace ECommerceOS.AuthService.Api.Identity.Logout;

public static class Endpoint
{
    private const string Route = "/logout";

    public static RouteHandlerBuilder MapLogoutEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();
        
        var command = new LogoutCommand(userId);

        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToProblemDetails();
        }

        httpContext.ExpireRefreshTokenCookie();
        return Results.NoContent();
    }
}
