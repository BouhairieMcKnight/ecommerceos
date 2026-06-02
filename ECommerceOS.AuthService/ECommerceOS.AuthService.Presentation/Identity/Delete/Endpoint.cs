
using ECommerceOS.AuthService.Api.Http;
using ECommerceOS.AuthService.Application.Identity.Command.Delete;

namespace ECommerceOS.AuthService.Api.Identity.Delete;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapDeleteEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapDelete(Route, HandleAsync)
                .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();
        var command = new DeleteCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}