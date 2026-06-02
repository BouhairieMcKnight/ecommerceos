namespace ECommerceOS.PaymentService.Domain.Transactions;

public abstract class TransactionMetadata : BaseEntity<string>
{
    public TransactionId TransactionId { get; init; } = null!;
    [MaxLength(15)]
    public string Type { get; init; } = string.Empty;
}
