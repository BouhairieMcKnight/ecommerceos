namespace ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;

public class CancelTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ITransactionService transactionService)
    : ICommandHandler<CancelTransactionCommand, string>
{
    public async Task<Result<string>> Handle(
        CancelTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await transactionRepository.GetByIdAsync(request.TransactionId!, cancellationToken)
            .Bind(t => t.CancelTransaction(request.Reason))
            .BindAsync(async t =>
            {
                var result = await transactionService.CancelTransactionAsync(t.Id, t.Status, cancellationToken);
                return result.IsSuccess ?
                    Result<(Transaction transaction, string status)>.Success((t, result.Value!)) :
                    Result<(Transaction transaction, string status)>.Failure(result.Error!);
            })
            .TapAsync(async t => await transactionRepository.UpdateAsync(t.transaction, cancellationToken));

        return result.Match(
            success => Result<string>.Success(success.transaction.Status.ToString()),
            Result<string>.Failure);
    }
}