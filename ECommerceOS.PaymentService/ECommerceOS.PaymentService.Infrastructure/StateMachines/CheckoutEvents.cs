using ECommerceOS.Shared.DTOs;
using Address = ECommerceOS.Shared.ValueObjects.Address;

namespace ECommerceOS.PaymentService.Infrastructure.StateMachines;

public record CheckoutSessionCompleted
{
    public Guid IdempotencyKey { get; set; }
    public required TransactionId TransactionId { get; set; }
    public required OrderId OrderId { get; set; }
    public required string Status { get; set; }
    public required UserId CustomerId { get; set; }
    public required PaymentId CustomerPaymentId { get; set; }
    public required ECommerceOS.Shared.ValueObjects.Address ShippingAddress { get; set; }
    public List<CheckoutDto> LineItems { get; set; } = [];
}

public record PaymentIntentCompleted
{
    public required TransactionId TransactionId { get; set; }
}

public record CancelTransaction
{
    public UserId CustomerId { get; set; }
    public string Reason { get; set; }
    public TransactionId TransactionId { get; set; }
}

public record TransactionFailedCancel
{
    public TransactionId TransactionId { get; set; }
    public string Error { get; set; }
}

public record TransactionSuccessfullyCancelled
{
    public TransactionId TransactionId { get; set; }
    public string Status { get; set; }
}

public record CapturePayment
{
    public Guid IdempotencyKey { get; set; }
    public required TransactionId TransactionId { get; set; }
    public required OrderId OrderId { get; set; }
    public required UserId CustomerId { get; set; }
    public long Amount { get; set; }
    public required string Currency { get; set; }
}

public record PaymentIntentCapturable
{
    public long CapturableAmount { get; set; }
    public required string Currency { get; set; }
    public required TransactionId TransactionId { get; set; }
}
