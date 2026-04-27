using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Payment;

public record PaymentMethodConfirmed : ISpecificRecord, IIntegrationEvent
{
    public static readonly Schema _SCHEMA = Avro.Schema.Parse(
        """
        {
          "type": "record",
          "name": "PaymentMethodConfirmed",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
          "fields": [
            { "name": "PaymentId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "PaymentMethodId", "type": "string" },
            { "name": "PaymentMethodType", "type": "string" },
            { "name": "Version", "type": "int" },
            { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
          ]
        }
        """);
    
    public required PaymentId PaymentId { get; set; }
    public required string PaymentMethodId { get; set; }
    public required string PaymentMethodType { get; set; }
    public int Version { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
    
    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => PaymentId.Value.ToString(),
            1 => PaymentMethodId,
            2 => PaymentMethodType,
            3 => Version,
            4 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: PaymentId = new PaymentId(Guid.Parse((string)fieldValue)); break;
            case 1: PaymentMethodId = (string)fieldValue; break;
            case 2: PaymentMethodType = (string)fieldValue; break;
            case 3: Version = (int)fieldValue; break;
            case 4: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _SCHEMA;
}