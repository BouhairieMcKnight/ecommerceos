using Microsoft.AspNetCore.Http;

namespace ECommerceOS.CatalogService.Application.Products.Command.UploadProductImages;

public record UploadProductImagesCommand(
    Guid IdempotentCommandId,
    IReadOnlyCollection<IFormFile> Files,
    ProductId ProductId,
    List<string> ImageNames) : ICommand, IIdempotentCommand;

    
    
    
