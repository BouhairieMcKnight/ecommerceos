using System.ComponentModel.DataAnnotations;

namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;

public sealed class StripeTransactionMetadata : TransactionMetadata
{
    public const string TypeValue = "stripe";
    [MaxLength(50)] public string StripeCustomerId { get; private set; } = string.Empty;


    private StripeTransactionMetadata()
    {
    }

    public static StripeTransactionMetadata Create(
        TransactionId transactionId,
        string transactionNumber,
        string stripeCustomerId)
    {
        return new StripeTransactionMetadata
        {
            Id = transactionNumber,
            TransactionId = transactionId,
            StripeCustomerId = stripeCustomerId,
            Type = TypeValue
        };
    }
}
