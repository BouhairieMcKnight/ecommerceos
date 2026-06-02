namespace ECommerceOS.CatalogService.Application.Products.Query.GetProduct;

public record GetProductQuery(ProductId ProductId) : IQuery<GetProductResponse>, ICachedQuery
{
    public string CacheKey => ProductId.ToString();
    public string Tag => nameof(Product);
    public TimeSpan CacheDuration { get; } = TimeSpan.FromMinutes(10);
}

public record GetProductResponse(
    ProductId ProductId,
    string Name,
    string Description,
    Money Price,
    int Quantity,
    IReadOnlyCollection<string> ImageUrls);
