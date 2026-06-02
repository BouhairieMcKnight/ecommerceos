using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceOS.CatalogService.Domain.Products;

public class Product : AggregateRoot<ProductId>, IAuditableEntity
{
    private readonly HashSet<string> _imageUrls = [];
    private readonly HashSet<ProductCategory>  _productCategories = [];
    public IEnumerable<ProductCategory> ProductCategories => _productCategories.AsReadOnly();
    [NotMapped]
    public IEnumerable<string> ImageUrls => _imageUrls.AsReadOnly();
    
    [MaxLength(100)]
    public string Description { get; private set; }
    public UserId SellerId { get; private set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public Sku Sku { get; private set; }
    public int Quantity { get; private set; }
    
    private Product()
    {
    }

    public static Result<Product> Create(
        UserId sellerId,
        Money price,
        Sku sku,
        string name,
        string description,
        int quantity,
        ProductId? productId = null)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            return Result<Product>.Failure(ProductErrors.NotValidProduct);
        }
        
        productId ??= new ProductId(Guid.NewGuid());
        
        var product = new Product
        {
            Id = productId,
            SellerId =  sellerId,
            Sku = sku,
            Price = price,
            Description =  description,
            Name = name,
            Quantity = quantity
        };
        
        return Result<Product>.Success(product); 
    }

    public Result UploadImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Result.Failure(ProductErrors.NotValidProduct);
        }
        
        var result = _imageUrls.Add(imageUrl);
        
        return result ?
            Result.Success()
            : Result.Failure(ProductErrors.NotValidProduct);
    }

    public Result<Product> DeleteProduct()
    {
        AddDomainEvent(new ProductDeletedDomainEvent(this)
        {
            OccurredOn = DateTimeOffset.UtcNow
        });
        
        return Result<Product>.Success(this);
    }

    public Result<Product> UpdatePrice(Money price)
    {
        if (price.Amount <= 0)
        {
            return Result<Product>.Failure(ProductErrors.NotValidProduct);
        }
        
        Price = price;
        return Result<Product>.Success(this);
    }

    public Result<Product> UpdateCategory(CategoryId categoryId) 
    {
        var result = _productCategories.All(pc => pc.CategoryId != categoryId);

        if (!result)
        {
            return Result<Product>.Failure(ProductErrors.NotValidProduct);
        }
        
        var productCategory = ProductCategory.Create(categoryId, Id);
        _productCategories.Add(productCategory);
        return result ? Result<Product>.Success(this) : Result<Product>.Failure(ProductErrors.NotValidProduct);
    }

    public Result UpdateDescription(string description)
    {
        Description = description;
        return Result.Success();
    }
}
