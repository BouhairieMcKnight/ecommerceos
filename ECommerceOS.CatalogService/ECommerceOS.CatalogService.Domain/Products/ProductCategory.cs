namespace ECommerceOS.CatalogService.Domain.Products;

public class ProductCategory : BaseEntity<ProductCategoryId>
{
    public CategoryId CategoryId { get; private set; }
    public ProductId ProductId { get; private set; }

    private ProductCategory()
    {
    }

    public static ProductCategory Create(CategoryId categoryId, ProductId productId)
    {
        var id = new  ProductCategoryId(Guid.NewGuid());
        
        return new ProductCategory
        {
            Id = id,
            CategoryId = categoryId,
            ProductId = productId
        };
    }
}