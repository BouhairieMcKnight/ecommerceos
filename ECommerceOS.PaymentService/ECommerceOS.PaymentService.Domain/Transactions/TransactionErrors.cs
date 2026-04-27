namespace ECommerceOS.PaymentService.Domain.Transactions;

public static class TransactionErrors
{
    public static readonly Error NotFound = Error.NotFound("Transaction.NotFound", "Transaction not found");
    
    public static readonly Error NotValidTransaction = Error.Validation("Transaction.Validation",
        "transaction is not valid");
    
    public static readonly Error NotValidOperation = Error.Validation("Operation.Validation",
        "transaction operation is not valid");

    public static Error NotFoundByTransactionId(TransactionId transactionId) => Error.NotFound("Transaction.NotFoundByTransactionId",
        $"No transaction found associated with {transactionId}");
}