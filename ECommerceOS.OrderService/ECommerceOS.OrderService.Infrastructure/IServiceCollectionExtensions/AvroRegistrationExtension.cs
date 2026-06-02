using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.OrderService.Infrastructure.Serialization;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using ECommerceOS.Shared.Contracts.Messaging.Order;

namespace ECommerceOS.OrderService.Infrastructure.IServiceCollectionExtensions;

public static class AvroRegistrationExtensions
{
    public static IServiceCollection AddAvroRegistrationExtension(this IServiceCollection services)
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
        
        services.AddSingleton<MultipleTypeConfig>( sp =>
        {
            var builder = new MultipleTypeConfigBuilder<IIntegrationEvent>();
            builder
                .AddType<CartCheckedOut>(CartCheckedOut._SCHEMA)
                .AddType<InventoryReserveFailed>(InventoryReserveFailed._SCHEMA)
                .AddType<OrderSubmitted>(OrderSubmitted._SCHEMA)
                .AddType<OrderConfirmed>(OrderConfirmed._SCHEMA)
                .AddType<OrderCancelled>(OrderCancelled._SCHEMA);

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

        return services;
    }
}
