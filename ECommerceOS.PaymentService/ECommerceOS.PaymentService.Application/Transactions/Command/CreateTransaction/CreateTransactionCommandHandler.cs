namespace ECommerceOS.PaymentService.Application.Transactions.Command.CreateTransaction;

public class CreateTransactionCommandHandler(ITransactionRepository transactionRepository) : ICommandHandler<CreateTransactionCommand>
{
    public async Task<Result> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = await Transaction.Create(
                request.CustomerId,
                request.Status,
                request.CustomerPaymentId,
                request.ShippingAddress,
                transactionId: request.TransactionId)
            .Bind(t => AddLineItems(t, request.TransactionLineItems))
            .TapAsync(async t => await transactionRepository.AddAsync(t, cancellationToken));

        return result.Match(
            _ => Result.Success(),
            Result.Failure);
    }
    
    private static Result<Transaction> AddLineItems(
        Transaction transaction,
        IEnumerable<TransactionLineItem> lineItems)
    {
        var errors = lineItems
            .Select(item => transaction.AddTransactionItem(item.SellerId, item.ProductId, item.Cost))
            .Where(result => !result.IsSuccess)
            .Select(result => result.Error).ToArray();
        
        return errors.Any()
            ? Result<Transaction>.Failure(errors!)
            : Result<Transaction>.Success(transaction);
    }
}