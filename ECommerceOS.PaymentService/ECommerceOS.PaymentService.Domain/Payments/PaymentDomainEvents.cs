namespace ECommerceOS.PaymentService.Domain.Payments;

public record PaymentAddedDomainEvent : IDomainEvent
{
    public string Type => nameof(Payment);
    public required UserId UserId { get; init; }
    public required PaymentId PaymentId { get; init; }
    public required PaymentMetadata PaymentMetadata { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record PaymentMethodRemovedDomainEvent : IDomainEvent
{
    public string Type => nameof(Payment);
    public required UserId UserId { get; init; }
    public required PaymentId PaymentId { get; init; }
    public required PaymentMetadata PaymentMetadata { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record PaymentStatusChangedDomainEvent : IDomainEvent
{
    public string Type => nameof(Payment);
    public required UserId UserId { get; init; }
    public required  PaymentId PaymentId { get; init; }
    public required PaymentStatus PaymentStatus { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}