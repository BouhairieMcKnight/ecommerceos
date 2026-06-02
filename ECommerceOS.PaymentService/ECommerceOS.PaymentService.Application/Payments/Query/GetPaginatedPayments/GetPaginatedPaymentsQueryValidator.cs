namespace ECommerceOS.PaymentService.Application.Payments.Query.GetPaginatedPayments;

public class GetPaginatedPaymentsQueryValidator : AbstractValidator<GetPaginatedPaymentsQuery>
{
    public GetPaginatedPaymentsQueryValidator(IPaymentRepository paymentRepository)
    {
        RuleFor(q => q.CustomerId)
            .NotNull()
            .WithMessage("Customer Id cannot be null");
        
        RuleFor(c => c.CustomerId)
            .NotNull()
            .MustAsync(async (c, ct) => 
                await paymentRepository.VerifyUserPaymentAsync(c!, ct))
            .When(q => q.CustomerId is not null)
            .WithMessage("User must have registered payment");

        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
