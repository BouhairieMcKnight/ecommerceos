using ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Presentation.Transaction.Cancel;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapCancelEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromBody] string reason,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var transactionId = new TransactionId(Guid.Parse(id));
        var userId = httpContext.GetUserId();
        var command = new CancelTransactionCommand
        {
            TransactionId = transactionId,
            Reason = reason,
            CustomerId= userId
        };
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}