namespace ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;

public class CancelTransactionCommandValidator : AbstractValidator<CancelTransactionCommand>
{
    public CancelTransactionCommandValidator(ITransactionRepository transactionRepository)
    {
        RuleFor(command => command.CustomerId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(command => command.TransactionId)
            .NotNull()
            .WithMessage("TransactionId cannot be null");
        
        RuleFor(c => c)
            .MustAsync(async (c, ct) => 
                await transactionRepository.VerifyTransactionAsync(c.TransactionId, c.CustomerId,  ct))
            .WithMessage("Order not found");
    }
}