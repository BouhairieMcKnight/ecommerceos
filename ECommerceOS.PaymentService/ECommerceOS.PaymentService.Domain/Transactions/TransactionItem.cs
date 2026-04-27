namespace ECommerceOS.PaymentService.Domain.Transactions;

public class TransactionItem : BaseEntity<TransactionItemId>
{
    public TransactionId TransactionId { get; internal set; } = null!;
    public UserId SellerId { get; internal set; } = null!;
    public ProductId ProductId { get; private set; } = null!;
    public Money Amount { get; internal set; } = null!;
    public int Quantity { get; internal set; }

    private TransactionItem()
    {
    }

    public static TransactionItem Create(
        UserId sellerId,
        ProductId productId,
        TransactionId transactionId,
        Money amount)
    {
        var id = new TransactionItemId(Guid.NewGuid());
        var transactionItem = new TransactionItem
        {
            Id = id,
            TransactionId = transactionId,
            SellerId = sellerId,
            ProductId = productId,
            Amount = amount
        };
        
        return transactionItem;
    }
}