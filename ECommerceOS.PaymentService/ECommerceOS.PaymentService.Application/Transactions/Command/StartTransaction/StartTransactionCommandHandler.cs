namespace ECommerceOS.PaymentService.Application.Transactions.Command.StartTransaction;

public class StartTransactionCommandHandler(
    ITransactionService transactionService)
    : ICommandHandler<StartTransactionCommand, string>
{
    public async Task<Result<string>> Handle(
        StartTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await transactionService.CreateCheckoutAsync(
            request.UserId!,
            request.CheckoutDtos,
            cancellationToken);

        return Result<string>.Success(result);
    }
}