using ECommerceOS.PaymentService.Application.Payments.Command.InitializePayment;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Presentation.Payment.InitializePayment;

public static class Endpoint
{
    private const string Route = "/add-payment";

    public static RouteHandlerBuilder MapInitializePaymentEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "X-Idempotency-Key")] string requestKey,
        [FromBody] string paymentMethod,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();

        if (!Guid.TryParse(requestKey, out Guid idempotencyKey))
        {
            return Results.BadRequest();
        }
        
        var command = new InitializePaymentCommand(idempotencyKey, userId, paymentMethod);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.Redirect(result.Value!) : result.ToProblemDetails();
    }
    
    
}