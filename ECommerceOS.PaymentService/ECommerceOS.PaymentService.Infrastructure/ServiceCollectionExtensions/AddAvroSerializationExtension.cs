using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Avro;
using ECommerceOS.PaymentService.Infrastructure.Messaging.Serialization;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Payment;

namespace ECommerceOS.PaymentService.Infrastructure.ServiceCollectionExtensions;

public static class AddAvroSerializationExtension
{
    public static void AddAvroSerialization(this IServiceCollection services)
    {
        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("schemaRegistry");

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = connectionString
            };
            
            var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            return schemaRegistryClient;
        });
        
        services.AddSingleton<MultipleTypeConfig>(_ =>
        {
            var builder = new MultipleTypeConfigBuilder<IIntegrationEvent>();
            builder
                .AddType<PaymentMethodConfirmed>(PaymentMethodConfirmed._SCHEMA)
                .AddType<TransactionSubmitted>(TransactionSubmitted._SCHEMA)
                .AddType<TransactionFailed>(TransactionFailed._SCHEMA)
                .AddType<TransactionConfirmed>(TransactionConfirmed._SCHEMA)
                .AddType<TransactionRefunded>(TransactionRefunded._SCHEMA)
                .AddType<TransactionRefundSubmitted>(TransactionRefundSubmitted._SCHEMA);

            return builder.Build();
        });

        services.AddSingleton<IAsyncDeserializer<IIntegrationEvent>>(sp =>
        {
            var config = sp.GetRequiredService<MultipleTypeConfig>();
            var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();

            return new MultipleTypeDeserializer<IIntegrationEvent>(config, schemaRegistry);
        });

        services.AddSingleton<IAsyncSerializer<IIntegrationEvent>>(sp =>
        {
            var config = sp.GetRequiredService<MultipleTypeConfig>();
            var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();
            
            var serializerConfig = new AvroSerializerConfig
            {
                SubjectNameStrategy = SubjectNameStrategy.Record,
                AutoRegisterSchemas = true
            };

            return new MultipleTypeSerializer<IIntegrationEvent>(config, schemaRegistry, serializerConfig);
        });
    }
}
