using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.DTOs;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Catalog;

public record CartCheckedOut : ISpecificRecord, IIntegrationEvent
{
    public static readonly Schema _SCHEMA = Avro.Schema.Parse(
        """
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
            { "name": "CreatedAt", "type": "long" }
          ]
        }
        """);
    
    public required CartId CartId { get; set; }
    public required UserId CustomerId { get; set; }
    public required List<CheckoutDto> Checkouts { get; set; }
    public int Version { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
    
    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => CartId.Value.ToString(),
            1 => CustomerId.Value.ToString(),
            2 => Checkouts,
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
            case 1: CustomerId = new UserId(Guid.Parse((string)fieldValue)); break;
            case 2: Checkouts = (List<CheckoutDto>)fieldValue; break;
            case 3: Version = (int)fieldValue; break;
            case 4: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _SCHEMA;
}