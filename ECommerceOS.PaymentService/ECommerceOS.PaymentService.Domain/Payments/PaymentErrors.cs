namespace ECommerceOS.PaymentService.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error NotFound = Error.NotFound("Payment.NotFound", "Payment not found");
    
    public static readonly Error NotValidPayment = Error.Validation("Payment.Validation",
        "Payment is not valid");
    
    public static Error NotFoundByPaymentId(PaymentId paymentId) => Error.NotFound("Payment.NotFoundByPaymentId",
        $"No payment found associated with {paymentId}");
    
    public static Error NotValidOperation(string code, string description) => Error.Failure(code, description); 
}