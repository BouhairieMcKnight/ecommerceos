namespace ECommerceOS.CatalogService.Presentation.Product.Post;

public static class Endpoint
{
    private const string Route = "/create-product";

    public static RouteHandlerBuilder MapProductPostEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromBody] ProductRequest productRequest,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var sellerId = httpContext.GetUserId();
        if (sellerId is null)
        {
            return Results.Unauthorized();
        }

        var command = new CreateProductCommand(
            SellerId: sellerId,
            ProductName: productRequest.Name,
            Description: productRequest.Description,
            Price: productRequest.Price,
            Quantity: productRequest.Quantity,
            Filenames: productRequest.Filenames,
            DirectImagesUpload: false);
        
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}

internal record ProductRequest(string Name, string Description, Money Price, int Quantity, List<string> Filenames);
