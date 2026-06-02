namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;

public sealed class IdempotentRequest
{
    public Guid Id  { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
}
