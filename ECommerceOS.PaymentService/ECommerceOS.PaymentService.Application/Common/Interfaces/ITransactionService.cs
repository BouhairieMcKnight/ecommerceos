using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface ITransactionService
{
    Task<string> CreateCheckoutAsync(
        UserId userId, IEnumerable<CheckoutDto> products, CancellationToken cancellationToken = default);

    Task<string> RefundCheckoutAsync(
        TransactionId transactionId,  CancellationToken cancellationToken = default);
    
    Task<Result<string>> CancelTransactionAsync(
        TransactionId transactionId,
        TransactionStatus status,
        CancellationToken cancellationToken = default);
    Task<string> GetTransactionStatusAsync(string sessionId, CancellationToken cancellationToken = default);

    Task CapturePaymentAsync(TransactionId transactionId,
        long amount,
        Guid idempotencyKey,
        CancellationToken cancellationToken = default);
}
