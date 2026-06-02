namespace ECommerceOS.PaymentService.Application.Transactions.EventHandlers;

public class TransactionCapturedNotification : INotification
{
    public TransactionId TransactionId { get; set; }
}

public class TransactionCapturedNotificationHandler(ITransactionRepository transactionRepository)
    : INotificationHandler<TransactionCapturedNotification>
{
    public async Task Handle(TransactionCapturedNotification notification, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository
            .GetByIdAsync(notification.TransactionId, cancellationToken)
            .Bind(t => t.ChangeTransactionStatus("Confirmed"))
            .TapAsync(async t => await transactionRepository.UpdateAsync(t, cancellationToken));
    }
}