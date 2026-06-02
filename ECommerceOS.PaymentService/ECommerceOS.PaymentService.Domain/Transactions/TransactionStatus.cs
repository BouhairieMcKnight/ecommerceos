namespace ECommerceOS.PaymentService.Domain.Transactions;

[Flags]
public enum TransactionStatus
{
    Pending = 1 << 0,
    Confirmed = 1 << 1,
    Completed = 1 << 2,
    Refunding = 1 << 3,
    Refunded = 1 << 4,
    Cancelled = 1 << 5
}

public static class TransactionStatusExtensions
{
    public static int GetLatest(this TransactionStatus status)
    {
        var value = (int)status;
        var setValues = Enum.GetValues(typeof(TransactionStatus)).Cast<int>()
            .Where(f => (f & value) == f).ToArray();
        
        return setValues.Length > 0 ? setValues.Max() : 0;
    }
}