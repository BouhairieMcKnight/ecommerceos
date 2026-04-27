using AvroSchemaGenerator.Attributes;
using MediatR;

namespace ECommerceOS.Shared.Contracts.Interfaces;

public interface IIntegrationEvent : INotification
{
    public DateTime CreatedAt { get; set; }
    public int Version { get; set; }
}