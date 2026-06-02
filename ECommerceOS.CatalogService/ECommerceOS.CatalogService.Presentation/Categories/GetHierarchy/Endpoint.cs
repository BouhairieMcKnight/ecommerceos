using ECommerceOS.CatalogService.Application.Categories.Query.GetCategoryHierarchy;

namespace ECommerceOS.CatalogService.Presentation.Categories.GetHierarchy;

public static class Endpoint
{
    private const string Route = "/hierarchy";

    public static RouteHandlerBuilder MapGetCategoryHierarchyEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetCategoryHierarchyQuery(), cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value!) : result.ToProblemDetails();
    }
}