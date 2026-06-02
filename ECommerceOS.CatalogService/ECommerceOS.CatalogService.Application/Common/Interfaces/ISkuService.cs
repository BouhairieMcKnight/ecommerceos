namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ISkuService
{
    Result<Sku> GenerateSku(string description);
}