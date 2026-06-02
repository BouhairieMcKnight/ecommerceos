namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransaction;

public class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
{
    public GetTransactionQueryValidator(ITransactionRepository transactionRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(c => c.TransactionId)
            .NotNull()
            .WithMessage("Transaction Id cannot be null");
        
        RuleFor(c => c)
            .MustAsync(async (c, ct) => 
                await transactionRepository.VerifyTransactionAsync(c.TransactionId!, c.UserId!,  ct))
            .When(c => c.UserId is not null && c.TransactionId is not null)
            .WithMessage("Order not found");
    }
}