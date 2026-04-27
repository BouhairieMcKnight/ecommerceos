namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Model;

internal sealed class IdempotentRequest
{
    public Guid Id  { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
}