namespace ECommerceOS.OrderService.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error NotFound = Error.NotFound("Order.NotFound", "Order not found");
    
    public static readonly Error NotValidOrder = Error.Validation("Order.Validation", "Order is not valid");
    
    public static Error NotValidOperation(string status) => Error.Validation("Order.Validation",
        $"Cannot change order status {status}");
    
    public static Error NotFoundByOrderId(OrderId orderId) => Error.NotFound("Order.NotFoundByOrderId",
        $"No order found associated with {orderId}");
}