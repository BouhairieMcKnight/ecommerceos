namespace ECommerceOS.PaymentService.Domain.Payments;

public enum PaymentStatus
{
    Pending,
    Processing,
    Validated,
    Failed
}