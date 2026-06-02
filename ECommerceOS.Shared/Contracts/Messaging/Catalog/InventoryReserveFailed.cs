using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Catalog;

public record InventoryReserveFailed : ISpecificRecord, IIntegrationEvent
{
    public static readonly Schema _SCHEMA = Schema.Parse(
        """
        {
          "type": "record",
          "name": "InventoryReserved",
          "displayName" : "Product Reserved",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Catalog",
          "fields": [
            { "name": "CartId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "Version", "type": "int" },
            { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
          ]
        }
        """);
    
    public required OrderId OrderId { get; set; }
    public required CartId CartId { get; set; }
    public TransactionId? TransactionId { get; set; }
    public int Version { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
    
    public object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => CartId.Value.ToString(),
            1 => OrderId.Value.ToString(),
            2 => TransactionId?.Value.ToString(),
            3 => Version,
            4 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: CartId = new CartId(Guid.Parse((string)fieldValue)); break;
            case 1: OrderId = new OrderId(Guid.Parse((string)fieldValue)); break;
            case 2: Version = (int)fieldValue; break;
            case 3: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _SCHEMA;
}