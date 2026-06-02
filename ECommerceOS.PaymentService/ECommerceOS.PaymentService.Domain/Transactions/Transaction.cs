using System.Diagnostics.CodeAnalysis;
using ECommerceOS.Shared.Contracts.Messaging.Payment;

namespace ECommerceOS.PaymentService.Domain.Transactions;

public class Transaction : AggregateRoot<TransactionId>, IAuditableEntity
{
    private readonly HashSet<TransactionItem> _transactionItems = [];
    public IEnumerable<TransactionItem> TransactionItems => _transactionItems.AsReadOnly();
    public OrderId OrderId { get; private set; } = null!;
    public TransactionMetadata TransactionMetadata { get; private set; } = null!;

    private bool CreationActive([NotNullWhen(true)] out TransactionCreatedDomainEvent? @event)
    {
        var exists = domainEvents.FirstOrDefault(e => e is TransactionCreatedDomainEvent);
        
        if (exists is TransactionCreatedDomainEvent created)
        {
            @event = created;
            return true;
        }
        
        @event = null;
        return false;
    }

    public PaymentId CustomerPaymentId { get; private set; } = null!;
    public TransactionStatus Status { get; private set; }
    public UserId CustomerId { get; private set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
    
    private Transaction()
    {
    }

    public static Result<Transaction> Create(
        UserId customerId,
        string status,
        PaymentId paymentId,
        Address address,
        OrderId? orderId = null,
        TransactionId? transactionId = null)
    {
        if (!Enum.TryParse<TransactionStatus>(status, out var transactionStatus))
        {
            return Result<Transaction>.Failure(TransactionErrors.NotValidOperation);
        }
        
        transactionId ??= new TransactionId(Guid.NewGuid());
        
        var transaction = new Transaction
        {
            Id =  transactionId,
            CustomerId = customerId,
            Status = transactionStatus,
            CustomerPaymentId = paymentId,
            OrderId = orderId ?? new OrderId(Guid.NewGuid())
        };

        transaction.AddDomainEvent(new TransactionCreatedDomainEvent
        {
            TransactionId = transactionId,
            CustomerPaymentId = paymentId,
            OrderId = orderId ?? new OrderId(Guid.NewGuid()),
            Address =  address,
            CustomerId = customerId,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Transaction>.Success(transaction);
    }

    public Result<Transaction> CancelTransactionItems(TransactionItemId itemId)
    {
        return 1 != _transactionItems.RemoveWhere(t => t.Id == itemId) ?
            Result<Transaction>.Failure(TransactionErrors.NotValidOperation) :
            Result<Transaction>.Success(this);
    }

    public Result<Transaction> AddTransactionItem(
        UserId sellerId,
        ProductId productId,
        Money cost)
    {
        if (!CreationActive(out TransactionCreatedDomainEvent? @event))
        {
            return Result<Transaction>.Failure(TransactionErrors.NotValidOperation);
        }
        
        var item = TransactionItem.Create(sellerId, productId, Id, cost);
        
        _transactionItems.Add(item);
        @event.TransactionItems.Add(item);
        
        return Result<Transaction>.Success(this);
    }

    public Result<Transaction> ChangeTransactionStatus(string status)
    {
        if (!Enum.TryParse<TransactionStatus>(status, out var transactionStatus) ||
            (int)transactionStatus < ((Status.GetLatest()) << 1))
        {
            return Result<Transaction>.Failure(TransactionErrors.NotValidOperation);
        }
        
        Status |= transactionStatus;
        
        AddDomainEvent(new TransactionStatusChangedDomainEvent
        {
            TransactionId = Id,
            TransactionStatus = transactionStatus,
            CustomerPaymentId = CustomerPaymentId,
            OrderId = OrderId,
            CustomerId = CustomerId,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Transaction>.Success(this);
    }

    public Result<Transaction> CancelTransaction(string reason)
    {
        if (Status.HasFlag(TransactionStatus.Refunded) ||
            Status.HasFlag(TransactionStatus.Refunding))
        {
            return Result<Transaction>.Failure(TransactionErrors.NotValidOperation);
        }
        
        if (Status.HasFlag(TransactionStatus.Confirmed) || Status.HasFlag(TransactionStatus.Completed))
        { 
            return Refund(reason);
        }

        Status |= TransactionStatus.Cancelled;
        
        AddDomainEvent(new TransactionStatusChangedDomainEvent
        {
            TransactionId = Id,
            TransactionStatus = Status,
            CustomerPaymentId = CustomerPaymentId,
            OrderId = OrderId,
            CustomerId = CustomerId,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Transaction>.Success(this);
    }

    private Result<Transaction> Refund(string reason)
    {
        AddDomainEvent(new TransactionStartedRefundDomainEvent
        {
            TransactionId = Id,
            CustomerPaymentId = CustomerPaymentId,
            OrderId = OrderId,
            CustomerId = CustomerId,
            Reason = reason,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Transaction>.Success(this);
    }
}
