using ECommerceOS.PaymentService.Application.Transactions.Command.StartTransaction;
using ECommerceOS.PaymentService.Application.Transactions.Query.GetTransactionStatus;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.DTOs;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Presentation.Transaction.InitializeTransaction;

public static class Endpoint
{
    private const string Route = "/create-checkout-session";

    public static RouteHandlerBuilder MapCreateSessionEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "X-Idempotency-Key")] string requestKey,
        [FromBody] IEnumerable<CheckoutDto> transactionProductDtos,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(requestKey, out var idempotencyKey))
        {
            return Results.BadRequest();
        }
        
        var userId = httpContext.GetUserId();
        var command = new StartTransactionCommand(idempotencyKey, userId, transactionProductDtos);
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Results.Redirect(result.Value!) : result.ToProblemDetails();
    }
}