using ECommerceOS.CatalogService.Application.Products.Query.GetProducts;

namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICatalogRepository : IRepository<Product, ProductId>
{
    Task<bool> VerifyProductAsync(ProductId id, CancellationToken cancellationToken);
    Task<bool> VerifySkuAsync(Sku sku, CancellationToken cancellationToken = default);
    Task<bool> VerifySellerProductAsync(ProductId id, UserId userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Product>>> GetProductsAsync(Sku sku, CancellationToken cancellationToken = default);
    Task UpdateProductsAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
    Task<Result<IQueryable<Product>>> GetProductsPaginatedAsync(Expression<Func<Product, object>> columnSelector,
        CategoryId? searchCategory,
        string? sortOrder,
        string? searchTerm,
        CancellationToken cancellationToken = default);

    Task<Result> ReserveInventoryAsync(Dictionary<ProductId, int> products, CancellationToken cancellationToken = default);
}