namespace ECommerceOS.CatalogService.Infrastructure.Inventory;

public class InventoryService(ICatalogRepository catalogRepository): ICartService
{
    public async Task<bool> CheckProductStock(ProductId productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await catalogRepository.GetByIdAsync(productId, cancellationToken);

        return product.IsSuccess && product.Value!.Quantity >= quantity;
    }

}
