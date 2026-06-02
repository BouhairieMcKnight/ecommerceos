using ECommerceOS.CatalogService.Application.Carts.Commands.CheckoutCart;

namespace ECommerceOS.CatalogService.Presentation.Carts.Checkouts;

public static class Endpoint
{
    private const string Route = "checkout";

    public static RouteHandlerBuilder MapPostCheckoutEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "X-Idempotency-Key")] string requestKey,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(requestKey, out Guid idempotencyKey))
        {
            return Results.BadRequest();
        }

        var userId = httpContext.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }
        
        var command = new CheckoutCartCommand(idempotencyKey, userId);
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}
