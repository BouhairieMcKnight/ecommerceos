namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransaction;

public record GetTransactionQuery(UserId? UserId, TransactionId? TransactionId)
    : IQuery<Transaction>, ICachedQuery
{
    public string Tag => nameof(Transaction);
    public string CacheKey => $"{UserId?.Value}-{TransactionId?.Value}";
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(10);
}
