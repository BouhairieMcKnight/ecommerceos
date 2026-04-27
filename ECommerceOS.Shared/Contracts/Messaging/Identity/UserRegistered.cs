using Avro;
using Avro.Specific;
using Avro.Util;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.Shared.Contracts.Messaging.Identity;

public record UserRegistered : IIntegrationEvent, ISpecificRecord
{
    public static readonly Schema _SCHEMA = Schema.Parse(
        """
        {
          "type": "record",
          "name": "UserRegistered",
          "displayName": "User Registered",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Identity",
          "fields": [
            { "name": "UserId", "type": ["null", "string"], default: null },
            { "name": "Email", "type": "string" },
            { "name": "Name", "type": "string" },
            { "name": "Version", "type": "int" },
            { "name": "CreatedAt", "type": "long" }
          ]
        }
        """);
    
    
    public UserId? UserId { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => UserId?.Value.ToString(),
            1 => Email,
            2 => Name,
            3 => Version,
            4 => CreatedAt.ToBinary(),
            _ => throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()")
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: UserId = new UserId(Guid.Parse((string)fieldValue)); break;
            case 1: Email = (string)fieldValue; break;
            case 2: Name = (string)fieldValue; break;
            case 3: Version = (int)fieldValue; break;
            case 4: CreatedAt = DateTime.FromBinary((long)fieldValue); break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }

    public virtual Schema Schema => _SCHEMA;
}