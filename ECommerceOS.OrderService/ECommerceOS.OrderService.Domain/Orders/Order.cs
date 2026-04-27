using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ECommerceOS.OrderService.Domain.Orders;

public class Order : AggregateRoot<OrderId>, IAuditableEntity
{
    private readonly HashSet<OrderItem> _orderItems = [];
    public IEnumerable<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public bool Completed => _orderItems.All(item => item.OrderItemStatus.HasFlag(OrderStatus.Delivered));

    public UserId CustomerId { get; private init; } = null!;
    public TransactionId TransactionId { get; private init; } = null!;
    public Address Address { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    
    private bool CreationActive([NotNullWhen(true)] out OrderCreatedDomainEvent? @event)
    {
        var exists = domainEvents.FirstOrDefault(e => e is OrderCreatedDomainEvent);
        
        if (exists is OrderCreatedDomainEvent created)
        {
            @event = created;
            return true;
        }
        
        @event = null;
        return false;
    }
    
    [NotMapped]
    public Money TotalPrice => _orderItems.
        Select(item => item.Price).
        Aggregate((price1, price2) => price1 + price2);
    
    private Order()
    {
    }

    public static Result<Order> Create(
        Address address,
        UserId customerId,
        TransactionId transactionId,
        string status,
        OrderId? orderId = null)
    {
        orderId ??= new OrderId(Guid.NewGuid());
        var order = new Order
        {
            Id = orderId,
            Status = Enum.TryParse<OrderStatus>(status, true, out var parsedStatus)
                ? parsedStatus
                : OrderStatus.Pending,
            Address = address,
            CustomerId = customerId,
            TransactionId = transactionId
        };
        
        order.AddDomainEvent(OrderCreatedDomainEvent.Create(order.Id, order.CustomerId, address, transactionId, status));
        
        return Result<Order>.Success(order);
    }

    public Result<Order> AddOrderItem(
        ProductId productId,
        int quantity,
        Money itemPrice,
        UserId sellerId,
        string imageUrl,
        OrderItemId? orderItemId = null)
    {
        if (!CreationActive(out var @event))
        {
            return Result<Order>.Failure(Error.None);
        }
        
        var orderItem = OrderItem.Create(
            productId: productId,
            quantity: quantity,
            price: itemPrice,
            orderId: Id,
            sellerId: sellerId,
            imageUrl: imageUrl,
            orderItemId: orderItemId);

        if (orderItem is null)
        {
            return Result<Order>.Failure(Error.None); 
        }
        
        var result = _orderItems.Add(orderItem);

        if (result)
        {
            @event.OrderItems.Add(orderItem);
        }
        
        return result ?
            Result<Order>.Success(this) : Result<Order>.Failure(OrderErrors.NotValidOrder);
    }
    
    public Result<Order> RemoveOrderItem(OrderItemId itemId)
    {
        var result = _orderItems.RemoveWhere(item => item.Id == itemId);

        if (result == 1)
        {
            AddDomainEvent(OrderItemRemovedDomainEvent.Create(Id, itemId));
        }
        
        return result == 1 ?
            Result<Order>.Success(this) : Result<Order>.Failure(OrderErrors.NotValidOrder);
    }

    public Result ChangeOrderItemStatus(OrderItemId itemId, string status)
    {
        var item = _orderItems.FirstOrDefault(item => item.Id == itemId);
        
        if (item is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        var result = item.ChangeStatus(status);

        if (result.IsSuccess)
        {
            AddDomainEvent(OrderItemStatusChangedDomainEvent
                .Create(Id, item.OrderItemStatus, item.Id));
            
            return Result.Success();
        }
        
        return Result.Failure(OrderErrors.NotValidOperation(status));
    }

    public Result<Order> CancelOrder()
    {
        Status = OrderStatus.Cancelled;
        AddDomainEvent(OrderCancelDomainEvent.Create(
            Id,
            CustomerId,
            TransactionId,
            _orderItems.ToList()));
        return Result<Order>.Success(this);
    }

    public Result<Order> ConfirmOrder(DateTimeOffset expectedDeliveryDate)
    {
        Status = OrderStatus.Delivering;

        var confirmed = OrderConfirmedDomainEvent.Create(
            Id,
            CustomerId,
            Address,
            TransactionId,
            expectedDeliveryDate);

        foreach (var item in _orderItems)
        {
            confirmed.OrderItems.Add(item);
        }

        AddDomainEvent(confirmed);
        return Result<Order>.Success(this);
    }
}
