using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.CatalogService.Infrastructure.Messaging.Middleware;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using MassTransit;

namespace ECommerceOS.CatalogService.Infrastructure.IServiceCollectionExtensions;

public static class MessagingRegistrationExtensions
{
    public static void AddMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(options =>
        {
            options.SetInMemorySagaRepositoryProvider();
            
            options.SetEntityFrameworkSagaRepositoryProvider(cfg =>
            {
                cfg.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                cfg.UsePostgres()
                    .ExistingDbContext<CatalogDbContext>();
            });
            
            options.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            options.AddRider(rider =>
            {
                rider.UsingKafka((context, cfg) =>
                {
                    cfg.Host(context);
                    cfg.Acks = Acks.All;
                    cfg.ClientId = "catalog-service";
                    var groupId = Guid.NewGuid().ToString();
                    
                    cfg.TopicEndpoint<string, OrderEvent>("order-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        endpoint.ConcurrentMessageLimit = 10;
                        endpoint.ConcurrentConsumerLimit = 2;
                        endpoint.ConcurrentDeliveryLimit = 1;

                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<OrderEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<OrderEvent>(m => m.IntegrationEvent!);
                    });
                });
            });
        });
    }

    private static void Host(this IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("kafka");
        configurator.Host(connectionString);
    }
}
