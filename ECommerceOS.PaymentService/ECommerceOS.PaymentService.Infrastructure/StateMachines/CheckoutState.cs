using ECommerceOS.Shared.DTOs;
using MassTransit;
using Address = ECommerceOS.Shared.ValueObjects.Address;

namespace ECommerceOS.PaymentService.Infrastructure.StateMachines;

public record CheckoutState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public TransactionId TransactionId { get; set; }
    public string CurrentState { get; set; }
    public OrderId OrderId { get; set; }
    public PaymentId PaymentId { get; set; }
    public UserId CustomerId { get; set; }
    public Guid? IdempotencyKey { get; set; }
    public required Address ShippingAddress { get; set; }
    public string TransactionStatus { get; set; } = string.Empty;
    public List<CheckoutDto> TransactionLineItems { get; set; } = [];
    public long TotalAmount { get; set; }
    public long CapturableAmount { get; set; }
    public string Currency { get; set; } = "usd";
    public int ReadyEventStatus { get; set; }

    public bool ValidateCapturable => CapturableAmount >= TotalAmount;
}
