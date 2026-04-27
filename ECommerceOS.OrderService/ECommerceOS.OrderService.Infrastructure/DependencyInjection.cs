using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.OrderService.Infrastructure.Background;
using ECommerceOS.OrderService.Infrastructure.Caching;
using ECommerceOS.OrderService.Infrastructure.EmailService;
using ECommerceOS.OrderService.Infrastructure.Grpc;
using ECommerceOS.OrderService.Infrastructure.Idempotency;
using ECommerceOS.OrderService.Infrastructure.IServiceCollectionExtensions;
using ECommerceOS.ReservationService;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using Microsoft.AspNetCore.Builder;
using Quartz;

namespace ECommerceOS.OrderService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddAvroRegistrationExtension();
        builder.Services.AddOptions<SmtpClientOptions>().Bind(builder.Configuration.GetSection(nameof(SmtpClientOptions)));
        builder.AddKafkaProducer<string, OrderEvent>("kafka", (sp, config) =>
        {
            var serializerConfig = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true,
                SubjectNameStrategy = SubjectNameStrategy.Record
            };
            
            var schemaRegistryClient = sp.GetRequiredService<ISchemaRegistryClient>();
            config.SetValueSerializer(new AvroSerializer<OrderEvent>(schemaRegistryClient, serializerConfig));
        });
        
        builder.Services.AddMessagingServices();

        builder.AddPersistence();
        
        builder.Services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(OutBoxPublisher));

            configure
                .AddJob<OutBoxPublisher>(jobKey)
                .AddTrigger(trigger =>
                    trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10)
                                .RepeatForever()));
        });
        builder.Services.AddQuartzHostedService(); 
        
        builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();

        builder.Services.AddGrpcClient<Reserve.ReserveClient>("reserve", static options =>
        {
            options.Address = new Uri("https+http://catalogservice");
        });
        
        builder.Services.AddScoped<InventoryReservationService>();

        builder.AddRedisDistributedCache("cache");
        builder.Services.AddFusionCache()
            .WithStackExchangeRedisBackplane();
        builder.Services.AddScoped<ICacheService, CacheService>();
    }
}
