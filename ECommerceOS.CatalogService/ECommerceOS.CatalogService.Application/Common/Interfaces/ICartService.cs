namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICartService
{
    Task<bool> CheckProductStock(ProductId productId, int quantity, CancellationToken cancellationToken = default);
}