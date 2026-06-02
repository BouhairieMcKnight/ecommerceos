namespace ECommerceOS.CatalogService.Application.Products.Command.DeleteProduct;

public record DeleteProductCommand(ProductId? ProductId, UserId? UserId) : ICommand;