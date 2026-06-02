namespace ECommerceOS.OrderService.Domain.Orders;

public class OrderItem : BaseEntity<OrderItemId>
{
    public OrderId OrderId { get; internal set; }
    public UserId SellerId { get; private set; }
    public int Quantity { get; internal set; }
    public string ImageUrl { get; internal set; }
    public Money Price { get; internal set; }
    public ProductId ProductId { get; internal set; }
    public OrderStatus OrderItemStatus { get; internal set; }

    private OrderItem()
    {
    }
    
    public static OrderItem? Create(
        ProductId productId,
        int quantity,
        Money price,
        OrderId orderId,
        UserId sellerId,
        string imageUrl,
        OrderItemId? orderItemId = null)
    {
        orderItemId ??= new OrderItemId(Guid.NewGuid());
        var orderItem = new OrderItem
        {
            Id =  orderItemId,
            SellerId =  sellerId,
            ProductId = productId,
            Quantity = quantity,
            Price = price,
            ImageUrl =  imageUrl,
            OrderId = orderId,
            OrderItemStatus = OrderStatus.Pending
        };

        return orderItem;
    }

    public Result ChangeStatus(string status)
    {
        if (!Enum.TryParse(status, out OrderStatus orderStatus) ||
            (int)orderStatus < ((OrderItemStatus.GetLatest()) << 1))
        {
            return Result.Failure(OrderErrors.NotValidOperation(status));
        }

        OrderItemStatus = orderStatus;
        return Result.Success();
    }
}