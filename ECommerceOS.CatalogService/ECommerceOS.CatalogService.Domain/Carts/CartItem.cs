namespace ECommerceOS.CatalogService.Domain.Carts;

public class CartItem : BaseEntity<CartItemId>
{
    public CartId CartId { get; init; }
    public int Quantity { get; internal set; }
    public Money Price { get; init; }
    public UserId SellerId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public ProductId ProductId { get; internal set; }
    public string ImageUrl { get; private set; }  = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private CartItem()
    {
    }

    public static CartItem Create(
        ProductId productId,
        CartId cartId,
        int quantity,
        string imageUrl,
        string description,
        Money price,
        string name)
    {
        var id = new CartItemId(Guid.NewGuid());
        
        var cartItem = new CartItem
        {
            Id = id,
            CartId = cartId,
            ProductId = productId,
            ImageUrl = imageUrl,
            Quantity = quantity,
            Price = price,
            Description =  description,
            Name =  name
        };
        
        return cartItem;
    }
}
