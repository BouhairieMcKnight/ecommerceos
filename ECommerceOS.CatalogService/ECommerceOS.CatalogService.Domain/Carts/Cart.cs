using ECommerceOS.Shared.DTOs;
using SharedCartId = ECommerceOS.Shared.ValueObjects.CartId;

namespace ECommerceOS.CatalogService.Domain.Carts;

public class Cart : AggregateRoot<SharedCartId>
{
    private readonly HashSet<CartItem> _cartItems = [];
    public IEnumerable<CartItem> CartItems => _cartItems.AsReadOnly();
    public int Count => _cartItems.Count();
    public UserId CustomerId { get; private set; }
    
    
    
    private Cart()
    {
    }

    public static Result<Cart> Create(UserId userId)
    {
        var id = new CartId(Guid.NewGuid());
        
        var cart = new Cart
        {
            Id = id,
            CustomerId = userId
        };
        
        return Result<Cart>.Success(cart);
    }

    public Result<Cart> AddItem(
        ProductId productId,
        Money price,
        string imageUrl,
        int quantity,
        string description,
        string name)
    {
        var cartItem = CartItem.Create(productId, Id, quantity, imageUrl, description, price, name);

        if (_cartItems.Add(cartItem))
        {
            return Result<Cart>.Success(this);
        }

        return Result<Cart>.Failure(Error.Failure("Add Cart Item", "Item already in cart"));
    }

    public void ClearCart()
    {
        var checkoutDtos = _cartItems.Select(item => new CheckoutDto
        {
            ProductId = item.ProductId,
            Cost = item.Price,
            ImageUrl = item.ImageUrl,
            Description = item.Description,
            Name = item.Name,
            Quantity = item.Quantity,
            SellerId = item.SellerId
        }).ToList();
        
        _cartItems.Clear();
        
        AddDomainEvent(new CartClearedDomainEvent
        {
            CartId = Id,
            CheckoutItems =  checkoutDtos,
            OccurredOn = DateTimeOffset.UtcNow
        });
    }

    public void Checkout()
    {
        var checkoutDtos = _cartItems.Select(item => new CheckoutDto
        {
            ProductId = item.ProductId,
            Cost = item.Price,
            ImageUrl = item.ImageUrl,
            Description = item.Description,
            Name = item.Name,
            Quantity = item.Quantity,
            SellerId = item.SellerId
        }).ToList();
        
        AddDomainEvent(new CartCheckoutDomainEvent
        {
            CartId = Id,
            CheckoutItems =  checkoutDtos,
            CustomerId = CustomerId,
            OccurredOn = DateTimeOffset.UtcNow
        });
    }
}
