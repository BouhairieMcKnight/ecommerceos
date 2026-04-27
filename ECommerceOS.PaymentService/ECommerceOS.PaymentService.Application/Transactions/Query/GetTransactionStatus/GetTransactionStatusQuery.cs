namespace ECommerceOS.PaymentService.Application.Transactions.Query.GetTransactionStatus;

public record GetTransactionStatusQuery(UserId? UserId, string? SessionId) 
    : IQuery<GetTransactionStatusResponse>;


public record GetTransactionStatusResponse(string Status)
{
    public static GetTransactionStatusResponse Create(string status)
    {
        return new GetTransactionStatusResponse(status);
    }
}