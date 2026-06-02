using ECommerceOS.AuthService.Api.Http;

namespace ECommerceOS.AuthService.Api.Identity.Login;

public static class Endpoint
{
    private const string Route = "/login";

    public static RouteHandlerBuilder MapLoginEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromBody] LoginCommand loginCommand,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(loginCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToProblemDetails();
        }
        
        httpContext.SetRefreshToken(result.Value!.RefreshToken);
        return Results.Ok(result.Value.AccessToken);
    }
}