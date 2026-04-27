namespace ECommerceOS.AuthService.Infrastructure.Persistence.Data.Models;

internal sealed class IdempotentRequest
{
    public Guid Id  { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
}