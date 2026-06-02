using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Domain.Tests.Carts;

public sealed class CartTests
{
    [Fact]
    public void AddItem_WithValidData_AddsItemToCart()
    {
        var createResult = Cart.Create(new UserId(Guid.NewGuid()));
        var cart = Assert.IsType<Cart>(createResult.Value);

        var addResult = cart.AddItem(
            new ProductId(Guid.NewGuid()),
            Money.Create(Currency.Usd, 15m)!,
            "https://cdn.example.com/2.png",
            2,
            "Description",
            "Name");

        Assert.True(addResult.IsSuccess);
        Assert.Equal(1, cart.Count);
        Assert.Single(cart.CartItems);
    }

    [Fact]
    public void ClearCart_WithExistingItems_ClearsItemsAndAddsDomainEvent()
    {
        var createResult = Cart.Create(new UserId(Guid.NewGuid()));
        var cart = Assert.IsType<Cart>(createResult.Value);
        var productId = new ProductId(Guid.NewGuid());

        cart.AddItem(
            productId,
            Money.Create(Currency.Usd, 20m)!,
            "https://cdn.example.com/3.png",
            1,
            "Test item",
            "Test");

        cart.ClearCart();

        Assert.Empty(cart.CartItems);
        var domainEvent = Assert.IsType<CartClearedDomainEvent>(Assert.Single(cart.DomainEvents));
        Assert.Equal(cart.Id, domainEvent.CartId);
        Assert.Single(domainEvent.CheckoutItems);
        Assert.Equal(productId, domainEvent.CheckoutItems[0].ProductId);
    }
}
