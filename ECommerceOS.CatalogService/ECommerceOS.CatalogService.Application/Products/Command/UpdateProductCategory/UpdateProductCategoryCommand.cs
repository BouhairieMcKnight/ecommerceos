namespace ECommerceOS.CatalogService.Application.Products.Command.UpdateProductCategory;

public record UpdateProductCategoryCommand(
    Sku Sku,
    CategoryId CategoryId) 
    : ICommand;