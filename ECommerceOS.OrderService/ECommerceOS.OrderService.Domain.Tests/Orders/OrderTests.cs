using ECommerceOS.OrderService.Domain.Orders;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.OrderService.Domain.Tests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_WithInvalidStatus_DefaultsToPendingAndRaisesCreatedEvent()
    {
        var result = Order.Create(
            CreateAddress(),
            new UserId(Guid.NewGuid()),
            new TransactionId(Guid.NewGuid()),
            "not-a-status");

        Assert.True(result.IsSuccess);
        var order = Assert.IsType<Order>(result.Value);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Contains(order.DomainEvents, e => e is OrderCreatedDomainEvent);
    }

    [Fact]
    public void AddOrderItem_WithCreationEvent_AddsItemAndUpdatesCreatedEventPayload()
    {
        var order = CreateOrder();

        var result = order.AddOrderItem(
            new ProductId(Guid.NewGuid()),
            quantity: 2,
            itemPrice: Money.Create(Currency.Usd, 20m)!,
            sellerId: new UserId(Guid.NewGuid()),
            imageUrl: "https://example.com/image.jpg");

        Assert.True(result.IsSuccess);
        Assert.Single(order.OrderItems);

        var createdEvent = Assert.IsType<OrderCreatedDomainEvent>(Assert.Single(order.DomainEvents));
        Assert.Single(createdEvent.OrderItems);
    }

    [Fact]
    public void AddOrderItem_WithoutCreationEvent_Fails()
    {
        var order = CreateOrder();
        order.ClearDomainEvents();

        var result = order.AddOrderItem(
            new ProductId(Guid.NewGuid()),
            quantity: 1,
            itemPrice: Money.Create(Currency.Usd, 10m)!,
            sellerId: new UserId(Guid.NewGuid()),
            imageUrl: "https://example.com/image.jpg");

        Assert.False(result.IsSuccess);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void ChangeOrderItemStatus_WithForwardTransition_SucceedsAndAddsDomainEvent()
    {
        var order = CreateOrder();
        order.AddOrderItem(
            new ProductId(Guid.NewGuid()),
            quantity: 1,
            itemPrice: Money.Create(Currency.Usd, 15m)!,
            sellerId: new UserId(Guid.NewGuid()),
            imageUrl: "https://example.com/image.jpg");
        var item = Assert.Single(order.OrderItems);

        var result = order.ChangeOrderItemStatus(item.Id, nameof(OrderStatus.Processing));

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Processing, item.OrderItemStatus);
        Assert.Contains(order.DomainEvents,
            e => e is OrderItemStatusChangedDomainEvent changed &&
                 changed.OrderItemId == item.Id &&
                 changed.OrderStatus == OrderStatus.Processing);
    }

    [Fact]
    public void OrderItem_ChangeStatus_WithBackwardTransition_Fails()
    {
        var item = OrderItem.Create(
            new ProductId(Guid.NewGuid()),
            quantity: 1,
            price: Money.Create(Currency.Usd, 5m)!,
            orderId: new OrderId(Guid.NewGuid()),
            sellerId: new UserId(Guid.NewGuid()),
            imageUrl: "https://example.com/image.jpg");

        Assert.NotNull(item);
        var moveForward = item.ChangeStatus(nameof(OrderStatus.Processing));
        Assert.True(moveForward.IsSuccess);

        var moveBackward = item.ChangeStatus(nameof(OrderStatus.Pending));

        Assert.False(moveBackward.IsSuccess);
        Assert.Equal(OrderStatus.Processing, item.OrderItemStatus);
    }

    private static Order CreateOrder()
    {
        var result = Order.Create(
            CreateAddress(),
            new UserId(Guid.NewGuid()),
            new TransactionId(Guid.NewGuid()),
            nameof(OrderStatus.Pending));

        Assert.True(result.IsSuccess);
        return Assert.IsType<Order>(result.Value);
    }

    private static Address CreateAddress() =>
        new("street", "city", "state", "country", "zip");
}
