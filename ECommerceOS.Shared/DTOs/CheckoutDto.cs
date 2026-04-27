using Avro;
using Avro.Specific;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.DTOs;

public record CheckoutDto : ISpecificRecord
{
    public required ProductId ProductId { get; set; }
    public required UserId SellerId { get; set; }
    public string ImageUrl { get; set; } =  string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required Money Cost { get; set; }
    public int Quantity { get; set; }

    private static readonly Schema _schema = Avro.Schema.Parse(
        """
        {
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
        """);
    
    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => ProductId.Value.ToString(),
            1 => SellerId.Value.ToString(),
            2 => ImageUrl,
            3 => Name,
            4 => Description,
            5 => Cost.ToString(),
            6 => Quantity,
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: ProductId = new ProductId(Guid.Parse((string)fieldValue)); break;
            case 1: SellerId = new UserId(Guid.Parse((string)fieldValue)); break;
            case 2: ImageUrl = (string)fieldValue; break;
            case 3: Name = (string)fieldValue; break;
            case 4: Description = (string)fieldValue; break;
            case 5: Cost = Money.Create((string)fieldValue); break;
            case 6: Quantity = (int)fieldValue; break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _schema;
}