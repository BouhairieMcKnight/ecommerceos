namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface ITransactionRepository : IRepository<Transaction, TransactionId>
{
    Task<bool> VerifyTransactionAsync(TransactionId transactionId, UserId userId, CancellationToken cancellationToken = default);
}