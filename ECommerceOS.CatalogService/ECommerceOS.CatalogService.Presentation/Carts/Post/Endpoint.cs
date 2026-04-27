using System.Text.Json;
using ECommerceOS.CatalogService.Application.Carts.Commands.AddCartItem;
using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Presentation.Carts.Post;

public static class Endpoint
{
    private const string Route = "/";

    public static RouteHandlerBuilder MapPostCartItemEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "X-Idempotency-Key")] string idempotentKey,
        [FromBody] Item item,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var session = httpContext.Session;
        var userId = httpContext.GetUserId();

        if (!Guid.TryParse(idempotentKey, out var idempotencyId))
        {
            return Results.BadRequest();
        }

        if (userId != null)
        {
            var command = new AddCartItemCommand
            {
                UserId = userId,
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                Price =  item.Price,
                ImageUrl = item.ImageUrl,
                IdempotentCommandId = idempotencyId
            };
            
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }

        await session.LoadAsync(cancellationToken);
        var cart = session.TryGetValue("cart", out var json) ?
            JsonSerializer.Deserialize<List<Item>>(json) :
            null;

        if (cart != null)
        {
            cart.Add(item);
            session.SetString("cart", JsonSerializer.Serialize(cart));
            return Results.NoContent();
        }
        
        session.SetString("cart", JsonSerializer.Serialize(new List<Item> { item }));
        return Results.NoContent();
    }

    internal record struct Item(
        string ImageUrl,
        Money Price,
        ProductId ProductId,
        string Name,
        string Description,
        int Quantity);
}
