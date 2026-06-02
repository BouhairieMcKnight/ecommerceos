namespace ECommerceOS.OrderService.Application.Orders.Command.CancelOrderItem;

public record CancelOrderItemCommand(OrderId OrderId, UserId? UserId, OrderItemId OrderItemId) : ICommand;