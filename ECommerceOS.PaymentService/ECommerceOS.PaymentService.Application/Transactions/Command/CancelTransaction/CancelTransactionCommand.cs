namespace ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;

public record CancelTransactionCommand : ICommand<string>
{
    public UserId? CustomerId { get; set; }
    public required TransactionId TransactionId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
    