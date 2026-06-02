using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.CatalogService.Infrastructure.Messaging.Serialization;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;

namespace ECommerceOS.CatalogService.Infrastructure.IServiceCollectionExtensions;

public static class AvroRegistrationExtensions
{
    public static IServiceCollection AddAvroRegistrations(this IServiceCollection services)
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
                .AddType<InventoryReserved>(InventoryReserved._SCHEMA);

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