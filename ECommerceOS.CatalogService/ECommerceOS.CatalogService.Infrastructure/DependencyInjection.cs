using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.CatalogService.Infrastructure.Auth;
using ECommerceOS.CatalogService.Infrastructure.CatalogServices;
using ECommerceOS.CatalogService.Infrastructure.EmailService;
using ECommerceOS.CatalogService.Infrastructure.External;
using ECommerceOS.CatalogService.Infrastructure.IServiceCollectionExtensions;
using ECommerceOS.CheckoutService;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI.Services;
using Quartz;

namespace ECommerceOS.CatalogService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddAvroRegistrations();
        builder.AddKafkaProducer<string, CatalogEvent>("kafka", (context, producerConfig) =>
        {
            var serializerConfig = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true,
                SubjectNameStrategy = SubjectNameStrategy.Record
            };
            
            var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
            producerConfig.SetValueSerializer(new AvroSerializer<CatalogEvent>(schemaRegistryClient, serializerConfig));
        });
        builder.Services.AddMessaging();
        builder.Services.AddSingleton(TimeProvider.System);
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
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<ICartService, InventoryService>();
        builder.Services.AddScoped<ISkuService, SkuService>();
        builder.Services.AddScoped<IBlobService, BlobService>();
        builder.Services.ConfigureOptions<JwtOptionsSetup>();
        builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
        builder.AddAzureBlobServiceClient("blobs");

        builder.AddRedisDistributedCache("cache");
        builder.Services.AddFusionCache()
            .WithStackExchangeRedisBackplane();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddGrpcClient<Checkout.CheckoutClient>("checkout",static options =>
        {
            options.Address = new Uri("https+http://paymentservice");
        });

        builder.Services.AddScoped<ICheckoutCartService, CheckoutCartService>();
        
        builder.Services.AddOptions<SmtpClientOptions>().Bind(builder.Configuration.GetSection(nameof(SmtpClientOptions)));
        builder.Services.AddTransient<IEmailSender, EmailSender>();
    }
}
