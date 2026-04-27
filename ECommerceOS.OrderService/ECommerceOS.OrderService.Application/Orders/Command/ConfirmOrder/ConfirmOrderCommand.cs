namespace ECommerceOS.OrderService.Application.Orders.Command.ConfirmOrder;

public record ConfirmOrderCommand(
    OrderId OrderId,
    DateTimeOffset ExpectedDeliveryDate) : ICommand;
