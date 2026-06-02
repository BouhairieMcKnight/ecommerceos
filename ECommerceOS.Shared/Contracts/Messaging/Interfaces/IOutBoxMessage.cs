using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Entity;

namespace ECommerceOS.Shared.Contracts.Messaging.Interfaces;

public interface IOutboxMessage
{
    Guid MessageId { get; set; }
    DateTime? ProcessedOn { get; set; }
    string Type { get; set; } 
    IIntegrationEvent? IntegrationEvent { get; set; }
    short Attempts { get; set; }
    string? ErrorMessage { get; set; }
    DateTime CreatedAt { get; set; }
}