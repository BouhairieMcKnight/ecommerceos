using ECommerceOS.PaymentService.Application.Transactions.Query.GetTransaction;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Presentation.Transaction.GetById;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapGetByIdEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute(Name = "id")] string transactionString,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var transactionId = new TransactionId(Guid.Parse(transactionString));
        
        var userId = httpContext.GetUserId();
        
        var command = new GetTransactionQuery(userId, transactionId);
        
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}