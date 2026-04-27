namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransaction;

public class GetTransactionQueryHandler(
    ITransactionRepository transactionRepository)
    : IQueryHandler<GetTransactionQuery, Transaction>
{
    public Task<Result<Transaction>> Handle(
        GetTransactionQuery request,
        CancellationToken cancellationToken)
    {
        return transactionRepository.GetByIdAsync(request.TransactionId!, cancellationToken);
    }
}
