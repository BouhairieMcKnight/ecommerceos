namespace ECommerceOS.OrderService.Application.Orders.Command.CancelOrder;

public record CancelOrderCommand(OrderId OrderId, UserId? UserId) : ICommand;
