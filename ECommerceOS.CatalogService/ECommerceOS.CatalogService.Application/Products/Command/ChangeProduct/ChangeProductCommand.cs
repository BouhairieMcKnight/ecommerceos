namespace ECommerceOS.CatalogService.Application.Products.Command.ChangeProduct;

public record ChangeProductCommand(
    ProductId ProductId,
    Money? Price = null,
    string? Description = null) 
    : ICommand<ChangeProductCommandResponse>;

public record ChangeProductCommandResponse(ProductId ProductId);