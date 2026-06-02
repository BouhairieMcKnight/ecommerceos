using System.Text.Json;
using ECommerceOS.CatalogService.Application.Carts.Queries.GetCart;
using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Presentation.Carts.GetCart;

public static class Endpoint
{
    private const string Route = "/";

    public static RouteHandlerBuilder MapGetCartEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var session = httpContext.Session;
        var userId = httpContext.GetUserId();

        if (userId != null)
        {
            var query = new GetCartQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }
        
        await session.LoadAsync(cancellationToken);
        var cart = session.TryGetValue("cart", out var json) ?
                JsonSerializer.Deserialize<List<Item>>(json) :
                null;

        if (cart != null)
        {
            return Results.Ok(cart);
        }
        
        session.SetString("cart", JsonSerializer.Serialize(Enumerable.Empty<Item>().ToList()));
        return Results.NoContent();
    }
    
    internal record struct Item(
        string ImageUrl,
        Money Price,
        ProductId ProductId,
        int Quantity);
}
