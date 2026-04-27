namespace ECommerceOS.CatalogService.Infrastructure.CatalogServices;

public class SkuService(ICatalogRepository catalogRepository) : ISkuService
{
    public Result<Sku> GenerateSku(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Sku>.Failure(ProductErrors.NotValidProduct);
        }

        var normalized = new string(description
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .Take(8)
            .ToArray());

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var suffix = Random.Shared.Next(1000000, 9999999).ToString();
            var candidate = (normalized + suffix).PadRight(15, 'X')[..15];
            var sku = Sku.Create(candidate);

            if (sku is null)
            {
                continue;
            }

            var exists = catalogRepository.VerifySkuAsync(sku).GetAwaiter().GetResult();
            if (!exists)
            {
                return Result<Sku>.Success(sku);
            }
        }

        return Result<Sku>.Failure(ProductErrors.OperationConflict(
            "SkuGeneration",
            "Could not generate a unique SKU."));
    }
}
