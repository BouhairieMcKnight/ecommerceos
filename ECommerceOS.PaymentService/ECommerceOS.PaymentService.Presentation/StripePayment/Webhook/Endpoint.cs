namespace ECommerceOS.PaymentService.Presentation.StripePayment.Webhook;

public static class Endpoint
{
    private const string Route = "/webhook-endpoint";

    public static RouteHandlerBuilder MapWebhookEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapPost(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromHeader(Name = "Stripe-Signature")] string stripeSignature,
        [FromServices] IStripeWebhookService stripeWebhookService,
        CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(httpContext.Request.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);
        var isValid = await stripeWebhookService.TryHandleAsync(json, stripeSignature, cancellationToken);
        return isValid ? Results.Ok() : Results.BadRequest();
    }
}
