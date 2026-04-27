namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransactionStatus;

public class GetTransactionStatusQueryValidator : AbstractValidator<GetTransactionStatusQuery>
{
    public GetTransactionStatusQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotNull()
            .WithMessage("User cannot be null");

        RuleFor(query => query.SessionId)
            .NotEmpty()
            .WithMessage("Session id is required");
    }
}
