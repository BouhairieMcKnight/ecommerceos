namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPayment;

public record GetPaymentQuery(
    UserId? UserId,
    PaymentId? PaymentId)
    : IQuery<GetPaymentQueryResponse>, ICachedQuery
{
    public string Tag => nameof(Payment);
    public string CacheKey => PaymentId?.ToString() ?? string.Empty;
    public TimeSpan CacheDuration { get; } = TimeSpan.FromMinutes(5);
}


public record GetPaymentQueryResponse(
    PaymentId PaymentId,
    UserId UserId,
    string PaymentNumber,
    string Method,
    string Status);
