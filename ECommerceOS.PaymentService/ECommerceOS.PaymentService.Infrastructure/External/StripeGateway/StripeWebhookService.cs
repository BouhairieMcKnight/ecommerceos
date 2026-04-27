namespace ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;

public sealed class StripeWebhookService(
    IOptions<StripeSettings> stripeOptions,
    StripeProcessor stripeProcessor) : IStripeWebhookService
{
    public async Task<bool> TryHandleAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default)
    {
        var webhookSecret = stripeOptions.Value.WebhookSecret;

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret, 300);
        }
        catch (Exception)
        {
            return false;
        }

        await stripeProcessor.HandleStripeEventAsync(stripeEvent, cancellationToken);
        return true;
    }
}
