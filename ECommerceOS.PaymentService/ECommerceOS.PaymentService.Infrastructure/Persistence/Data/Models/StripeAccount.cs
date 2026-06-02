namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;

public sealed class StripeAccount
{
    public UserId UserId { get; set; }
    public string AccountId { get; set; } = string.Empty;

    private StripeAccount()
    {
    }

    public static StripeAccount Create(UserId userId, string customerId)
    {
        var account = new StripeAccount
        {
            UserId = userId,
            AccountId = customerId
        };
        return account;
    }
}
