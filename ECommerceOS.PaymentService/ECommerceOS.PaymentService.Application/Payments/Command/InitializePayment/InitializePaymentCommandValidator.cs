namespace ECommerceOS.PaymentService.Application.Payments.Command.InitializePayment;

public class InitializePaymentCommandValidator : AbstractValidator<InitializePaymentCommand>
{
    public InitializePaymentCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
        
        RuleFor(c => c.PaymentMethod)
            .NotEmpty()
            .Must(p => Enum.TryParse<PaymentMethod>(p, true, out var _))
            .WithMessage("Valid payment method is required");
    }
}
