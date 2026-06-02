using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.OrderService.Application.Orders.Command.SubmitOrder;

public record SubmitOrderCommand(
    OrderId OrderId,
    UserId CustomerId,
    TransactionId TransactionId,
    Address ShippingAddress,
    IReadOnlyCollection<CheckoutDto> OrderItems) : ICommand;
