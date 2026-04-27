using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Payment;

public record TransactionFailed : IIntegrationEvent, ISpecificRecord
{ 
    public static readonly Schema _SCHEMA = Schema.Parse(
        """
        {
          "type": "record",
          "name": "TransactionFailed",
          "displayName": "Transaction Failed",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
          "fields": [
            { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
            { "name": "Reason", "type": "string" },
            { "name": "Version", "type": "int" },
            { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
          ]
        }
        """);
    
    public required TransactionId TransactionId { get; set; }
    public required UserId CustomerId { get; set; }
    public required string Reason { get; set; }
    public int Version { get; set; }
    [LogicalType(LogicalTypeKind.Date)]
    public DateTime CreatedAt { get; set; }
    
    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => TransactionId.Value.ToString(),
            1 => CustomerId.Value.ToString(),
            2 => Reason,
            3 => Version,
            4 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: TransactionId = new TransactionId(Guid.Parse((string)fieldValue)); break;
            case 1: CustomerId = new UserId(Guid.Parse((string)fieldValue)); break;
            case 2: Reason = (string)fieldValue; break;
            case 3: Version = (int)fieldValue; break;
            case 4: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public Schema Schema => _SCHEMA;
}