namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPayment;

public class GetPaymentQueryValidator : AbstractValidator<GetPaymentQuery>
{
    public GetPaymentQueryValidator(IPaymentRepository paymentRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(command => command.PaymentId)
            .NotNull()
            .WithMessage("Payment Id cannot be null");
        
        RuleFor(c => c)
            .MustAsync(async (c, ct) => 
                await paymentRepository.VerifyPaymentAsync(c.PaymentId!, c.UserId!,  ct))
            .When(c => c.UserId is not null && c.PaymentId is not null)
            .WithMessage("Payment not found");
    }
}