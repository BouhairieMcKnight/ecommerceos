using ECommerceOS.CatalogService.Application.Products.Query.GetProduct;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.CatalogService.Presentation.Product.GetById;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapProductGetByIdEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellation = default)
    {
        var productId = new ProductId(Guid.Parse(id));
        var query = new GetProductQuery(productId);

        var result = await sender.Send(query, cancellation);

        return result.IsSuccess ? Results.Ok(result.Value!) : result.ToProblemDetails();
    }
}
