using System.ComponentModel.DataAnnotations;

namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;

public sealed class StripePaymentMetadata : PaymentMetadata
{
    public const string TypeValue = "stripe";

    [MaxLength(50)]
    public string StripeCustomerId { get; private set; } = string.Empty;

    private StripePaymentMetadata()
    {
    }

    public static StripePaymentMetadata Create(
        string paymentMethodId,
        string stripeCustomerId,
        PaymentId paymentId,
        string paymentMethod)
    {
        return new StripePaymentMetadata
        {
            Id = paymentMethodId,
            Type = TypeValue,
            StripeCustomerId = stripeCustomerId,
            PaymentId = paymentId,
            PaymentMethod = paymentMethod
        };
    }
}
