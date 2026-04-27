using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceOS.OrderService.Domain.Orders;

public record OrderCreatedDomainEvent : IDomainEvent
{
    public string Type => nameof(Order);
    public UserId CustomerId { get; init; } 
    public OrderId OrderId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    public Address Address { get; init; }
    public required string Status { get; init; }
    public TransactionId TransactionId { get; init; }
    
    [NotMapped] public readonly List<OrderItem> OrderItems = [];
    
    private OrderCreatedDomainEvent()
    {
    }
    
    public static OrderCreatedDomainEvent Create(
        OrderId orderId, UserId customerId, Address address, TransactionId transactionId, string status)
    {
        var orderEvent = new OrderCreatedDomainEvent
        {
            OrderId = orderId,
            CustomerId = customerId,
            OccurredOn= DateTimeOffset.Now,
            Address = address,
            Status = status,
            TransactionId = transactionId
        };
        
        return orderEvent;
    }
}

public record OrderConfirmedDomainEvent : IDomainEvent
{
    public string Type => nameof(Order);
    public UserId CustomerId { get; init; }
    public OrderId OrderId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    public DateTimeOffset ExpectedDeliveryDate { get; init; }
    public Address Address { get; init; }
    public TransactionId TransactionId { get; init; }
    
    [NotMapped] public readonly List<OrderItem> OrderItems = [];
    
    private OrderConfirmedDomainEvent()
    {
    }
    
    public static OrderConfirmedDomainEvent Create(
        OrderId orderId,
        UserId customerId,
        Address address,
        TransactionId transactionId,
        DateTimeOffset expectedDeliveryDate)
    {
        var orderEvent = new OrderConfirmedDomainEvent
        {
            OrderId = orderId,
            CustomerId = customerId,
            OccurredOn= DateTimeOffset.Now,
            Address = address,
            TransactionId = transactionId,
            ExpectedDeliveryDate = expectedDeliveryDate
        };
        
        return orderEvent;
    }
}

public record OrderItemAddedDomainEvent: IDomainEvent
{
    public string Type => nameof(Order);
    public UserId SellerId { get; init; }
    public OrderId OrderId { get; init; }
    public OrderItemId  OrderItemId { get; init; }
    public int Quantity { get; init; }
    public Money Price { get; init; }
    public UserId CustomerId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    public TransactionId TransactionId { get; init; }
    public ProductId ProductId { get; init; }
    private OrderItemAddedDomainEvent()
    {
    }
    
    public static OrderItemAddedDomainEvent Create(
        OrderId orderId,
        OrderItemId orderItemId,
        ProductId productId,
        UserId customerId,
        int quantity,
        Money price,
        UserId sellerId)
    {
        var orderEvent = new OrderItemAddedDomainEvent
        {
            OrderId = orderId,
            SellerId = sellerId,
            CustomerId =  customerId,
            OrderItemId = orderItemId,
            Quantity = quantity,
            ProductId =  productId,
            Price = price,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return orderEvent;
    } 
}

public record OrderItemRemovedDomainEvent: IDomainEvent
{
    public string Type => nameof(Order);
    public OrderId OrderId { get; init; }
    public OrderItemId OrderItemId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    private OrderItemRemovedDomainEvent()
    {
    }
    
    public static OrderItemRemovedDomainEvent Create(OrderId orderId, OrderItemId orderItemId)
    {
        var orderEvent = new OrderItemRemovedDomainEvent
        {
            OrderId = orderId,
            OrderItemId = orderItemId,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return orderEvent;
    } 
}

public record OrderCancelDomainEvent : IDomainEvent
{
    public string Type => nameof(Order);
    public OrderId OrderId { get; init; }
    public UserId CustomerId { get; init; }
    public TransactionId TransactionId { get; init; }
    public string? Reason { get; init; }
    [NotMapped] public readonly List<OrderItem> OrderItems = [];

    public DateTimeOffset OccurredOn { get; init; }

    private OrderCancelDomainEvent()
    {
    }

    public static OrderCancelDomainEvent Create(
        OrderId orderId,
        UserId customerId,
        TransactionId transactionId,
        IEnumerable<OrderItem> orderItems,
        string? reason = null)
    {
        var orderEvent = new OrderCancelDomainEvent
        {
            OrderId = orderId,
            CustomerId = customerId,
            TransactionId = transactionId,
            Reason = reason,
            OccurredOn = DateTimeOffset.UtcNow
        };

        orderEvent.OrderItems.AddRange(orderItems);
        return orderEvent;
    }
}

public record OrderItemStatusChangedDomainEvent : IDomainEvent
{
    public string Type => nameof(Order);
    public OrderId OrderId { get; init; }
    public OrderItemId OrderItemId { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    private OrderItemStatusChangedDomainEvent()
    {
    }
    
    public static OrderItemStatusChangedDomainEvent Create(OrderId orderId, OrderStatus orderStatus,
        OrderItemId orderItemId)
    {
        var orderEvent= new OrderItemStatusChangedDomainEvent
        {
            OrderId = orderId,
            OrderItemId = orderItemId,
            OrderStatus = orderStatus,
            OccurredOn= DateTimeOffset.Now,
        };
        
        return orderEvent;
    } 
}
