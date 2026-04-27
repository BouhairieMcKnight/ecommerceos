using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.OrderService.Infrastructure.Consumers;
using ECommerceOS.OrderService.Infrastructure.Middleware;
using ECommerceOS.OrderService.Infrastructure.StateMachines;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;

namespace ECommerceOS.OrderService.Infrastructure.IServiceCollectionExtensions;

public static class MessagingExtension
{
    public static void AddMessagingServices(this IServiceCollection services)
    {
        services.AddMassTransit(options =>
        {
            options.SetInMemorySagaRepositoryProvider();
            
            options.SetKebabCaseEndpointNameFormatter();
            options.AddConsumer<ProcessInventoryReservationConsumer>();
            options.AddConsumer<ProcessSubmitOrderConsumer>();
            options.AddConsumer<ProcessConfirmOrderConsumer>();
            options.AddConsumer<ProcessCancelOrderConsumer>();

            options.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
            
            options.AddRider(rider =>
            {
                rider.AddSagaStateMachine<OrderStateMachine, OrderState>();
                
                rider.UsingKafka((context, cfg) =>
                {
                    cfg.Host(context);
                    cfg.Acks = Acks.All;
                    cfg.ClientId = "order-service";
                    var groupId = Guid.NewGuid().ToString();
                    cfg.TopicEndpoint<string, PaymentEvent>("payment-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        
                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<PaymentEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<PaymentEvent>(m => m.IntegrationEvent);
                        endpoint.ConfigureSaga<OrderState>(context);
                    });

                    cfg.TopicEndpoint<string, CatalogEvent>("catalog-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        
                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<CatalogEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<CatalogEvent>(m => m.IntegrationEvent);
                        endpoint.ConfigureSaga<OrderState>(context);
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
