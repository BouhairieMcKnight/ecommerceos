namespace ECommerceOS.PaymentService.Application.Transactions.Command.StartTransaction;

public class StartTransactionCommandValidator : AbstractValidator<StartTransactionCommand>
{
    public StartTransactionCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
    }
}