using ECommerceOS.PaymentService.Domain.Transactions;
using ECommerceOS.Shared.ValueObjects;
using Xunit;

namespace ECommerceOS.PaymentService.Domain.Tests.Transactions;

public class TransactionTests
{
    [Fact]
    public void Create_WithInvalidStatus_ShouldFail()
    {
        var result = Transaction.Create(
            new UserId(Guid.NewGuid()),
            "Nope",
            new PaymentId(Guid.NewGuid()),
            BuildAddress());

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public void AddTransactionItem_WhenCreationEventMissing_ShouldFail()
    {
        var transaction = CreateTransaction();
        transaction.ClearDomainEvents();

        var result = transaction.AddTransactionItem(
            new UserId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            Money.Create(Currency.Usd, 19.99m)!);

        Assert.False(result.IsSuccess);
        Assert.Empty(transaction.TransactionItems);
    }

    [Fact]
    public void AddAndCancelTransactionItem_ShouldRemoveExactlyOneItem()
    {
        var transaction = CreateTransaction();

        var addResult = transaction.AddTransactionItem(
            new UserId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            Money.Create(Currency.Usd, 9.50m)!);

        Assert.True(addResult.IsSuccess);
        var item = Assert.Single(transaction.TransactionItems);

        var cancelResult = transaction.CancelTransactionItems(item.Id);

        Assert.True(cancelResult.IsSuccess);
        Assert.Empty(transaction.TransactionItems);
    }

    [Fact]
    public void ChangeTransactionStatus_WhenTransitionBackwards_ShouldFail()
    {
        var transaction = CreateTransaction(nameof(TransactionStatus.Confirmed));

        var result = transaction.ChangeTransactionStatus(nameof(TransactionStatus.Pending));

        Assert.False(result.IsSuccess);
        Assert.Equal(TransactionStatus.Confirmed, transaction.Status);
    }

    [Fact]
    public void CancelTransaction_WhenCompleted_ShouldStartRefund()
    {
        var transaction = CreateTransaction(nameof(TransactionStatus.Completed));

        var result = transaction.CancelTransaction("customer requested cancellation");

        Assert.True(result.IsSuccess);
        Assert.Contains(transaction.DomainEvents, e => e is TransactionStartedRefundDomainEvent);
        Assert.DoesNotContain(transaction.DomainEvents, e => e is TransactionStatusChangedDomainEvent s && s.TransactionStatus.HasFlag(TransactionStatus.Cancelled));
    }

    [Fact]
    public void CancelTransaction_WhenRefunding_ShouldFail()
    {
        var transaction = CreateTransaction(nameof(TransactionStatus.Refunding));

        var result = transaction.CancelTransaction("duplicate cancellation");

        Assert.False(result.IsSuccess);
    }

    private static Transaction CreateTransaction(string status = nameof(TransactionStatus.Pending))
    {
        var result = Transaction.Create(
            new UserId(Guid.NewGuid()),
            status,
            new PaymentId(Guid.NewGuid()),
            BuildAddress());

        return Assert.IsType<Transaction>(result.Value);
    }

    private static Address BuildAddress()
    {
        return new Address("1 Main", "Austin", "TX", "US", "78701");
    }
}
