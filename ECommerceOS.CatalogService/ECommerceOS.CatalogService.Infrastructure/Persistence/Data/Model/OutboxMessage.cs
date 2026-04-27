namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Model;

public record OutboxMessage
{
    public Guid MessageId { get; set; }
    public required string Type { get; set; }
    public required byte[] IntegrationEvent { get; set; } = [];
    public short Attempts { get; set; }
    public string? Error { get; set; }
    public DateTime? ProcessedOn { get; set; } = null;
    public DateTime CreatedAt { get; set; }
}