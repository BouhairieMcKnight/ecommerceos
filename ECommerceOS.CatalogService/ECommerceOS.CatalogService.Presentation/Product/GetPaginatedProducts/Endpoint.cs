using ECommerceOS.CatalogService.Application.Products.Query.GetProducts;

namespace ECommerceOS.CatalogService.Presentation.Product.GetPaginatedProducts;

public static class Endpoint
{
    private const string Route = "/";

    public static RouteHandlerBuilder MapGetPaginatedProductsEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [AsParameters] GetProductsQuery query,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value!) : result.ToProblemDetails();
    }
}