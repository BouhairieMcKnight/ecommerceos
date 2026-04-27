using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.PaymentService.Infrastructure.Consumers;
using ECommerceOS.PaymentService.Infrastructure.Messaging.Middleware;
using ECommerceOS.PaymentService.Infrastructure.StateMachines;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using ECommerceOS.Shared.Contracts.Messaging.Identity;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.ServiceCollectionExtensions;

public static class AddMessagingExtension
{
    public static void AddMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(options =>
        {
            options.SetInMemorySagaRepositoryProvider();
            
            options.AddConsumer<ProcessCapturePaymentConsumer>();
            options.AddConsumer<ProcessCancelTransactionConsumer>();
            options.AddConsumer<ProcessStripeAccountSetup>();
            options.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>();

            options.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            options.AddRider(rider =>
            {
                rider.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>();
                

                rider.AddConsumer<ProcessUserEmailVerificationConsumer>();

                rider.UsingKafka((context, cfg) =>
                {
                    cfg.Host(context);
                    cfg.Acks = Acks.All;
                    cfg.ClientId = "payment-service";
                    var groupId = Guid.NewGuid().ToString();
                    
                    cfg.TopicEndpoint<string, OrderEvent>("order-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        
                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<OrderEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<OrderEvent>(m => m.IntegrationEvent!);
                        
                        endpoint.ConfigureSaga<CheckoutState>(context);
                    });

                    cfg.TopicEndpoint<string, CatalogEvent>("catalog-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        
                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<CatalogEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<CatalogEvent>(m => m.IntegrationEvent!);
                        
                        endpoint.ConfigureSaga<CheckoutState>(context);
                    });
                    
                    cfg.TopicEndpoint<string, IdentityEvent>("identity-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        
                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<IdentityEvent>(schemaRegistryClient)
                        .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<IdentityEvent>(m => m.IntegrationEvent!);
                        endpoint.ConfigureConsumer<ProcessUserEmailVerificationConsumer>(context);
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
