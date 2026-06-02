using ECommerceOS.PaymentService.Application.Payments.Command.RemovePaymentMethod;
using ECommerceOS.PaymentService.Application.Transactions.Command.CancelTransaction;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.Result;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.PaymentService.Presentation.Payment.Cancel;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapDeleteEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapDelete(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var paymentId = new PaymentId(Guid.Parse(id));
        var userId = httpContext.GetUserId();
        var command = new RemovePaymentMethodCommand(userId, paymentId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}