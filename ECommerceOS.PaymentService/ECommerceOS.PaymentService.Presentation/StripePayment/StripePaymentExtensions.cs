using ECommerceOS.PaymentService.Presentation.StripePayment.Webhook;

namespace ECommerceOS.PaymentService.Presentation.StripePayment;

public static class StripePaymentExtensions
{
    private const string RoutePrefix = "/stripe";

    public static RouteGroupBuilder MapStripeGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);
        group.MapWebhookEndpoint();
        return group;
    }
}
