using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.OrderService.Infrastructure.StateMachines;

public record ReserveProduct
{
    public required ProductId ProductId { get; init; }
    public required int Quantity { get; init; }
}

public record ReserveInventory
{
    public required OrderId OrderId { get; init; }
    public required UserId CustomerId { get; init; }
    public required IReadOnlyCollection<ReserveProduct> ReserveProducts { get; init; }
}

public record CancelOrder
{
    public required OrderId OrderId { get; init; }
    public UserId? CustomerId { get; init; }
    public string? Reason { get; init; }
}

public record RefundTransaction
{
    public required OrderId OrderId { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required UserId CustomerId { get; init; }
    public required Money Amount { get; init; }
}

public record ConfirmOrder
{
    public required OrderId OrderId { get; init; }
    public required DateTimeOffset ExpectedDeliveryDate { get; init; }
}

public record ReserveFailed
{
    public required OrderId OrderId { get; init; }
    public string? Reason { get; init; }
}

public record ReserveSucceeded
{
    public required OrderId OrderId { get; init; }
}

public record SubmitOrder
{
    public required Address ShippingAddress { get; init; }
    public required OrderId OrderId { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required UserId CustomerId { get; init; }
    public required List<CheckoutDto> OrderItems { get; init; }
}

public record SubmitOrderSucceeded
{
    public required OrderId OrderId { get; init; }
}

public record SubmitOrderFailed
{
    public required OrderId OrderId { get; init; }
    public string? Reason { get; init; }
}

public record ConfirmOrderSucceeded
{
    public required OrderId OrderId { get; init; }
    public required DateTimeOffset ExpectedDeliveryDate { get; init; }
}

public record ConfirmOrderFailed
{
    public required OrderId OrderId { get; init; }
    public string? Reason { get; init; }
}
