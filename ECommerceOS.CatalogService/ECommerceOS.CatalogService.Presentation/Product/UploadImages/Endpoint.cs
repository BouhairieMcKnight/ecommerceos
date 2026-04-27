using ECommerceOS.CatalogService.Application.Products.Command.UploadProductImages;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.CatalogService.Presentation.Product.UploadImages;

public static class Endpoint
{
    private const string Route = "upload-images";

    public static RouteHandlerBuilder MapPutImagesEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPut(Route, HandleAsync);
    }
    
    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "X-Idempotency-Key")] string requestId,
        [FromForm] UploadImagesRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(requestId, out Guid idempotencyId) || request.Files.Count == 0)
        {
            return Results.BadRequest();
        }

        var command = new UploadProductImagesCommand(
            IdempotentCommandId: idempotencyId,
            Files: request.Files,
            ImageNames: request.ImageNames,
            ProductId: request.ProductId
        );
        
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
    }

    public sealed class UploadImagesRequest
    {
        public ProductId ProductId { get; init; } = new(Guid.Empty);
        public List<string> ImageNames { get; init; } = [];
        public List<IFormFile> Files { get; init; } = [];
    }
}
