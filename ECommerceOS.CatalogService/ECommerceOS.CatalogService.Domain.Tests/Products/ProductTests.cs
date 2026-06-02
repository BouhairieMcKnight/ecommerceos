using ECommerceOS.CatalogService.Domain.Products;

namespace ECommerceOS.CatalogService.Domain.Tests.Products;

public sealed class ProductTests
{
    [Fact]
    public void UploadImage_WithDuplicateUrl_ReturnsFailure()
    {
        var product = CreateProduct();
        var firstResult = product.UploadImage("https://cdn.example.com/1.png");

        var secondResult = product.UploadImage("https://cdn.example.com/1.png");

        Assert.True(firstResult.IsSuccess);
        Assert.False(secondResult.IsSuccess);
        Assert.Equal(ProductErrors.NotValidProduct, secondResult.Error);
    }

    [Fact]
    public void UpdateCategory_WhenCategoryAlreadyExists_ReturnsFailure()
    {
        var product = CreateProduct();
        var categoryId = new CategoryId(Guid.NewGuid());
        var firstResult = product.UpdateCategory(categoryId);

        var secondResult = product.UpdateCategory(categoryId);

        Assert.True(firstResult.IsSuccess);
        Assert.False(secondResult.IsSuccess);
        Assert.Single(product.ProductCategories);
    }

    private static Product CreateProduct()
    {
        var result = Product.Create(
            new UserId(Guid.NewGuid()),
            Money.Create(Currency.Usd, 10.5m)!,
            Sku.Create("123456789012345")!,
            "Test Product",
            "Test Description",
            3);

        return Assert.IsType<Product>(result.Value);
    }
}
