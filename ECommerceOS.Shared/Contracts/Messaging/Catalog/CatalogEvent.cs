using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Interfaces;

namespace ECommerceOS.Shared.Contracts.Messaging.Catalog;

public record CatalogEvent : ISpecificRecord, IOutboxMessage
{
    public static Schema _SCHEMA = Schema.Parse(
        """"
        {
          "type": "record",
          "name": "CatalogEvent",
          "displayName": "Catalog Event",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Catalog",
          "fields": [
            { "name": "MessageId", "displayName": "Event ID", "type": { "type": "string", "logicalType": "uuid" } },
            {
              "name": "ProcessedOn",
              "type": [
                "null",
                {
                  "type": "long",
                  "logicalType": "timestamp-millis"
                }
              ],
              "default": null
            },
            { "name": "Type", "type": "string" },
            { "name": "IntegrationEvent",
              "type": [
                {
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Catalog",
                  "type": "record",
                  "name": "CartCheckedOut",
                  "displayName": "Cart Checked Out",
                  "fields": [
                    { "name": "CartId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    {
                      "name": "Checkouts",
                      "displayName": "Checkout Items",
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
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "InventoryReserved",
                  "displayName" : "Inventory Reserved",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Catalog",
                  "fields": [
                    { "name": "CartId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                }
              ]
            },
            { "name": "Attempts", "type": "int" },
            { "name": "ErrorMessage", "type": ["null", "string"], "default": null },
            { "name": "CreatedAt", "type": "long" }
          ]
        }
        """");
    
    
    public object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => MessageId.ToString(),
            1 => ProcessedOn?.ToBinary(),
            2 => Type,
            3 => IntegrationEvent,
            4 => (int)Attempts,
            5 => ErrorMessage,
            6 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object? fieldValue)
    {
        switch (fieldPos)
        {
            case 0: MessageId = Guid.Parse((string)fieldValue!); break;
            case 1: ProcessedOn = fieldValue != null ? DateTime.FromBinary((long)fieldValue) : null; break;
            case 2: Type = (string)fieldValue!; break;
            case 3: IntegrationEvent = (IIntegrationEvent)fieldValue!; break;
            case 4: Attempts = (short)fieldValue!; break;
            case 5: ErrorMessage = (string?)fieldValue; break;
            case 6: CreatedAt = DateTime.FromBinary((long)fieldValue!); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _SCHEMA;
    public Guid MessageId { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public required string Type { get; set; }
    public IIntegrationEvent? IntegrationEvent { get; set; }
    public short Attempts { get; set; }
    public string? ErrorMessage { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
}