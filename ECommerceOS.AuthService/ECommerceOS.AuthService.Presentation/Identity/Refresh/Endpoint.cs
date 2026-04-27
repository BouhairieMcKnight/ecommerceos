using ECommerceOS.AuthService.Api.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ECommerceOS.AuthService.Api.Identity.Refresh;

public static class Endpoint
{
    private const string Route = "/refresh";

    public static RouteHandlerBuilder MapRefreshEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();
        var refreshToken = httpContext.GetRefreshToken();
        var command = new RefreshCommand(userId, refreshToken);

        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToProblemDetails();
        }

        httpContext.SetRefreshToken(result.Value!.RefreshToken);
        return Results.Ok(result.Value.AccessToken);
    }
}
