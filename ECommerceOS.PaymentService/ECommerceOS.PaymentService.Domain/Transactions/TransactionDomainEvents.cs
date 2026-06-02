using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceOS.PaymentService.Domain.Transactions;

public record TransactionCreatedDomainEvent : IDomainEvent
{
    public string Type => nameof(Transaction);
    public required TransactionId TransactionId { get; init; }
    public required PaymentId CustomerPaymentId { get; init; }
    public required UserId CustomerId { get; init; }
    public required OrderId OrderId { get; init; }
    public required Address Address { get; init; }
    public DateTimeOffset OccurredOn { get; init; }

    [NotMapped] public readonly List<TransactionItem> TransactionItems = [];
}

public record TransactionConfirmedDomainEvent : IDomainEvent
{
    public string Type => nameof(Transaction);
    public required PaymentId CustomerPaymentId { get; init; }
    public required OrderId OrderId { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required Address Address { get; init; }
    public required UserId CustomerId { get; init; }
    public required Money Total { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record TransactionCancelledDomainEvent : IDomainEvent
{
    public string Type => nameof(Transaction);
    public required PaymentId CustomerPaymentId { get; init; }
    public required string Reason { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required UserId CustomerId { get; init; }
    public required OrderId OrderId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record TransactionStartedRefundDomainEvent : IDomainEvent
{
    public string Type => nameof(Transaction);
    public required PaymentId CustomerPaymentId { get; init; }
    public required string Reason { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required UserId CustomerId { get; init; }
    public required OrderId OrderId { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record TransactionStatusChangedDomainEvent : IDomainEvent
{
    public string Type => nameof(Transaction);
    public required PaymentId CustomerPaymentId { get; init; }
    public required TransactionId TransactionId { get; init; }
    public required UserId CustomerId { get; init; }
    public required OrderId OrderId { get; init; }
    public TransactionStatus TransactionStatus { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}