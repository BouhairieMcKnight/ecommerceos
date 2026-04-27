namespace ECommerceOS.PaymentService.Application.Transactions.Command.CreateTransaction;

public class CreateTransactionCommand : ICommand
{
    public TransactionId TransactionId { get; set; }
    public string Status { get; set; }
    public Address ShippingAddress { get; set; }
    public PaymentId CustomerPaymentId { get; set; }
    public UserId CustomerId { get; set; }
    public IEnumerable<TransactionLineItem> TransactionLineItems { get; set; }
}

public record TransactionLineItem
{
    public UserId SellerId { get; set; }
    public Money Cost { get; set; }
    public ProductId ProductId { get; set; }
}