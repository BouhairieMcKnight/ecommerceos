namespace ECommerceOS.CatalogService.Application.Products.Query.GetProducts;

public record GetProductsQuery(
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    string? SearchCategory,
    int PageNumber,
    int PageSize) 
    : IQuery<PaginatedList<GetProductsQueryResponse>>, ICachedQuery
{
    public string CacheKey => $"{SearchCategory}_{SearchTerm}_{SortColumn}_{SortOrder}";
    public string Tag => nameof(Product);
    public TimeSpan CacheDuration { get; } = TimeSpan.FromMinutes(30);
}

public record GetProductsQueryResponse(
    ProductId ProductId,
    string ImageUrl,
    string Description,
    string Name,
    Money Price);