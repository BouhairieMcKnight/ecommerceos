using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.AuthService.Infrastructure.Background;
using ECommerceOS.AuthService.Infrastructure.Consumers;
using ECommerceOS.AuthService.Infrastructure.Messaging.Serialization;
using ECommerceOS.AuthService.Infrastructure.Middleware;
using ECommerceOS.AuthService.Infrastructure.Saga;


namespace ECommerceOS.AuthService.Infrastructure.ServiceCollectionExtensions;

public static class MessagingRegistrationExtensions
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(options =>
        {
            options.SetInMemorySagaRepositoryProvider();

            options.AddSagaStateMachine<OnBoardingStateMachine, OnBoardingState>();
            
            options.AddConsumer<ProcessUserVerificationConsumer>();
            
            options.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
                cfg.UseInMemoryOutbox();
            });
            
            options.AddRider(rider =>
            {
                rider.AddSagaStateMachine<OnBoardingStateMachine, OnBoardingState>();
                
                rider.UsingKafka((context, cfg) =>
                {
                    cfg.Host(context);
                    cfg.Acks = Acks.All;
                    cfg.ClientId = "auth-service";
                    
                    var groupId = Guid.NewGuid().ToString(); 
                    
                    cfg.TopicEndpoint<string, IdentityEvent>("identity-event", groupId, endpoint =>
                    {
                        endpoint.AutoOffsetReset = AutoOffsetReset.Earliest;
                        endpoint.ConcurrentMessageLimit = 10;
                        
                        // create up to two Confluent Kafka consumers, increases throughput with multiple partitions
                        endpoint.ConcurrentConsumerLimit = 1;
                        
                        endpoint.CheckpointInterval = TimeSpan.FromSeconds(1);
                        // delivery only one message per key value within a partition at a time (default)
                        endpoint.ConcurrentDeliveryLimit = 1;

                        var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
                        endpoint.SetValueDeserializer(new AvroDeserializer<IdentityEvent>(schemaRegistryClient)
                            .AsSyncOverAsync());
                        
                        endpoint.UseAvroUnionMessageTypeFilter<IdentityEvent>(m => m.IntegrationEvent!);
                        endpoint.ConfigureSaga<OnBoardingState>(context);
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
