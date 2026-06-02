using ECommerceOS.PaymentService.Application.Payments.Query.GetPayment;
using ECommerceOS.PaymentService.Presentation.Http;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.PaymentService.Presentation.Payment.Get;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapGetByIdEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }


    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();
        var paymentId = new PaymentId(Guid.Parse(id));
        var query = new GetPaymentQuery(userId, paymentId);
        
        var result = await sender.Send(query, cancellationToken);
        
        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}