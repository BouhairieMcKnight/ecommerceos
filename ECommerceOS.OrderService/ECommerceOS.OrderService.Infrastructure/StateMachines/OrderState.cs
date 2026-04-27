using ECommerceOS.Shared.DTOs;
using MassTransit;

namespace ECommerceOS.OrderService.Infrastructure.StateMachines;

public record OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Address ShippingAddress { get; set; } = Address.Create(",,,,");
    public UserId CustomerId { get; set; } = new(Guid.Empty);
    public TransactionId TransactionId { get; set; } = new(Guid.Empty);
    public List<CheckoutDto> CheckoutDtos { get; set; } = [];
    public Money Amount { get; set; } = Money.Create("USD", 0m)!;
    public OrderId OrderId { get; set; } = new(Guid.Empty);
    public CartId CartId { get; set; } = new(Guid.Empty);
    public string Status { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public DateTimeOffset ExpectedDeliveryDate { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
}
