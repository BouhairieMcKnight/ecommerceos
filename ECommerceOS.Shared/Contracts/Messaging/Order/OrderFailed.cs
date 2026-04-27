using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.DTOs;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Order;

public record OrderFailed : ISpecificRecord, IIntegrationEvent
{
    public static readonly Schema _SCHEMA = Avro.Schema.Parse(
        """
        {
          "type": "record",
          "name": "Order Failed",
          "displayName": "OrderFailed",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Order",
          "fields": [
            { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
            {
              "name": "OrderItems",
              "displayName": "Order Items",
              "type": {
                "type": "array",
                "items": {
                  "type": "record",
                  "name": "CheckoutDto",
                  "displayName": "Checkout Item",
                  "namespace": "ECommerceOS.Shared.DTOs",
                  "fields": [
                    { "name": "ProductId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "SellerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "ImageUrl", "type": "string" },
                    { "name": "Name", "type": "string" },
                    { "name": "Description", "type": "string" },
                    { "name": "Cost", "type": "string" },
                    { "name": "Quantity", "type": "int" }
                  ]
                }
              },
              "default": []
            },
            { "name": "Reason", "type": ["null", "string"], "default": "null" },
            { "name": "Version", "type": "int" },
            { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
          ]
        }
        """);
    
    public required TransactionId TransactionId { get; set; }
    public UserId? CustomerId { get; set; }
    public required List<CheckoutDto> OrderItems { get; set; }
    public required string Reason { get; set; }
    public int Version { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
        
    public object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => TransactionId.Value.ToString(),
            1 => CustomerId?.Value.ToString(),
            2 => OrderItems,
            3 => Reason,
            4 => Version,
            5 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: TransactionId = new TransactionId(Guid.Parse((string)fieldValue)); break;
            case 1: CustomerId = new UserId(Guid.Parse((string)fieldValue)); break;
            case 2: OrderItems = (List<CheckoutDto>)fieldValue; break;
            case 3: Reason = (string)fieldValue; break;
            case 4: Version = (int)fieldValue; break;
            case 5: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
    
    public Schema Schema => _SCHEMA;
}