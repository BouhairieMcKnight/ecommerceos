using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.AuthService.Infrastructure.Messaging.Serialization;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;

namespace ECommerceOS.AuthService.Infrastructure.ServiceCollectionExtensions;

public static class AvroRegistrationExtensions
{
    public static void AddAvroRegistrations(this IServiceCollection services)
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
        
        services.AddSingleton<MultipleTypeConfig>(static _ =>
        {
            var builder = new MultipleTypeConfigBuilder<IIntegrationEvent>();
            builder
                .AddType<UserRegistered>(UserRegistered._SCHEMA)
                .AddType<UserEmailVerified>(UserEmailVerified._SCHEMA);

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