namespace ECommerceOS.CatalogService.Domain.Products;

public static class ProductErrors
{
    public static readonly Error NotFound = Error.NotFound("Product.NotFound", "Product not found");
    
    public static readonly Error NotValidProduct = Error.Validation("Product.Validation",
        "Product is not valid");

    public static Error NotFoundBySku(Sku sku) => Error.NotFound("Product.NotFoundBySku",
        $"No product found associated with {sku}");
    
    public static Error OperationConflict(string code, string message) => Error.Conflict(code, message);
}