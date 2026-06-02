namespace ECommerceOS.OrderService.Infrastructure.Persistence.Data.Models;

public record OutboxMessage
{
    public Guid MessageId { get; set; }
    public required string Type { get; set; }
    public required byte[] IntegrationEvent { get; set; } = [];
    public short Attempts { get; set; }
    public string? Error { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public DateTime CreatedAt { get; set; }
}