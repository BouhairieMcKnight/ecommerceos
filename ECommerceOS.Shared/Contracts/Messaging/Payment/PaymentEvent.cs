using Avro;
using Avro.Specific;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Interfaces;

namespace ECommerceOS.Shared.Contracts.Messaging.Payment;

public record PaymentEvent : ISpecificRecord, IOutboxMessage
{
    public static readonly Schema _SCHEMA = Schema.Parse(
        """"
        {
          "type": "record",
          "name": "PaymentEvent",
          "displayName": "Payment Event",
          "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
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
                  "type": "record",
                  "name": "PaymentMethodConfirmed",
                  "displayName": "Payment Method Confirmed",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "PaymentId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "PaymentMethodId", "type": "string" },
                    { "name": "PaymentMethodType", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionFailed",
                  "displayName": "Transaction Failed",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "Reason", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionSubmitted",
                  "displayName": "Transaction Submitted",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    {
                      "name": "TransactionItems",
                      "displayName": "Transaction Items",
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
                    { "name": "Address", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionConfirmed",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "Total", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionRefunded",
                  "displayName": "Transaction Refunded",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    {
                      "name": "TransactionItems",
                      "displayName": "Transaction Items",
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
                    { "name": "Address", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionRefunded",
                  "displayName": "Transaction Refunded",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    {
                      "name": "TransactionItems",
                      "displayName": "Transaction Items",
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
                    { "name": "Address", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": { "type": "long", "logicalType": "timestamp-millis" } }
                  ]
                },
                {
                  "type": "record",
                  "name": "TransactionRefundSubmitted",
                  "displayName": "Transaction Refund Submitted",
                  "namespace": "ECommerceOS.Shared.Contracts.Messaging.Payment",
                  "fields": [
                    { "name": "TransactionId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "CustomerId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "OrderId", "type": { "type": "string", "logicalType": "uuid" } },
                    { "name": "Reason", "type": "string" },
                    {
                      "name": "TransactionItems",
                      "displayName": "Transaction Items",
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
                    { "name": "Address", "type": "string" },
                    { "name": "Version", "type": "int" },
                    { "name": "CreatedAt", "type": "long" }
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
    public DateTime CreatedAt { get; set; }
}