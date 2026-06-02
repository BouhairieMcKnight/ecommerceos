namespace ECommerceOS.PaymentService.Domain.Payments;

public class Payment : AggregateRoot<PaymentId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = null!;
    
    [MaxLength(32)]
    public string Name { get; private set; } = string.Empty;

    public PaymentMetadata PaymentMetadata { get; private set; } = null!;
    
    public PaymentStatus PaymentStatus { get; private set; }
    
    public DateTimeOffset CreatedOn { get; set; }
    
    public DateTimeOffset ModifiedOn { get; set; }

    private Payment()
    {
    }

    public static Result<Payment> Create(
        UserId userId,
        PaymentMetadata paymentMetadata,
        PaymentId? paymentId = null)
    {
        
        paymentId ??= new PaymentId(Guid.NewGuid());
        var payment = new Payment
        {
            Id = paymentId,
            UserId = userId,
            Name = paymentMetadata.PaymentMethod,
            PaymentMetadata = paymentMetadata,
            PaymentStatus = PaymentStatus.Pending
        };
        
        payment.AddDomainEvent(new PaymentAddedDomainEvent
        {
            PaymentId = payment.Id,
            UserId = payment.UserId,
            PaymentMetadata =  paymentMetadata,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Payment>.Success(payment);
    }

    public Result<Payment> ChangePaymentStatus(string paymentStatus)
    {
        if(!Enum.TryParse<PaymentStatus>(paymentStatus, out var status))
        {
            return Result<Payment>.Failure(PaymentErrors.NotValidOperation("Payment status", "Payment status is not valid"));
        }
        
        PaymentStatus = status;
        AddDomainEvent(new PaymentStatusChangedDomainEvent
        {
            PaymentId = Id,
            UserId = UserId,
            PaymentStatus = PaymentStatus,
            OccurredOn = DateTime.UtcNow
        });
        
        return Result<Payment>.Success(this);
    }

    public Result<Payment> RemovePayment()
    {
        AddDomainEvent( new PaymentMethodRemovedDomainEvent
        {
            PaymentId = Id,
            PaymentMetadata =  PaymentMetadata,
            UserId = UserId,
            OccurredOn = DateTime.UtcNow
        });
        return Result<Payment>.Success(this);
    }
}
