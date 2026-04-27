namespace ECommerceOS.PaymentService.Domain.Payments;

public abstract class PaymentMetadata : BaseEntity<string>
{
    [MaxLength(10)] public string PaymentMethod { get; init; } = string.Empty;
    public PaymentId PaymentId { get; init; } = null!;
    [MaxLength(15)] public string Type { get; init; } = string.Empty;
}
