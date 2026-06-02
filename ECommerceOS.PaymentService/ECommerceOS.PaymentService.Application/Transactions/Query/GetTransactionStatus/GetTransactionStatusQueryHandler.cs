namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransactionStatus;

public class GetTransactionStatusQueryHandler(
    ITransactionService transactionService) 
    : IQueryHandler<GetTransactionStatusQuery,
        GetTransactionStatusResponse>
{
    public async Task<Result<GetTransactionStatusResponse>> Handle(
        GetTransactionStatusQuery request,
        CancellationToken cancellationToken)
    {
        var result = await transactionService.GetTransactionStatusAsync(request.SessionId!, cancellationToken);

        return Result<GetTransactionStatusResponse>.Success(new GetTransactionStatusResponse(result));
    }
}