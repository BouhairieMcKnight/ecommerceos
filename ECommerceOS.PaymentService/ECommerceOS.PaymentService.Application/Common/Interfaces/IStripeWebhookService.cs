namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface IStripeWebhookService
{
    Task<bool> TryHandleAsync(string payload, string signature, CancellationToken cancellationToken = default);
}
