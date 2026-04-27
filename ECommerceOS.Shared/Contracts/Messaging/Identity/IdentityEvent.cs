using Avro;
using Avro.Specific;
using AvroSchemaGenerator.Attributes;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Interfaces;

namespace ECommerceOS.Shared.Contracts.Messaging.Identity;

public record IdentityEvent : ISpecificRecord, IOutboxMessage
{
    public static Schema _SCHEMA = Schema.Parse(
        """"
        {
          "type": "record",
          "name": "IdentityEvent",
          "displayName": "IdentityEvent",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Identity",
          "fields": [
            { "name": "MessageId", "displayName": "Event ID", "type": "string" },
            { "name": "ProcessedOn", "type": ["null", "long"], "default": null },
            { "name": "Type", "type": "string" },
            { "name": "IntegrationEvent",
              "type": [
                "null",
                {
                  "type": "record",
                  "name": "UserRegistered",
                  "displayName": "User Registered",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Identity",
                  "fields": [
                    { "name": "UserId", "type": "string" },
                    { "name": "Email", "type": "string" },
                    { "name": "Name", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": "long" }
                  ]
                },
                {
                  "type": "record",
                  "name": "UserEmailVerified",
                  "displayName": "User Email Verified",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Identity",
                  "fields": [
                    { "name": "UserId", "type": "string" },
                    { "name": "Email", "type": "string" },
                    { "name": "Name", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": "long" }
                  ]
                }
              ],
              "default": null
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
            case 4: Attempts = Convert.ToInt16((int)fieldValue!); break;
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
    public DateTime CreatedAt { get; set; }
}