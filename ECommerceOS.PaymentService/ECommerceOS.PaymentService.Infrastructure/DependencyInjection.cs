using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ECommerceOS.PaymentService.Infrastructure.Caching;
using ECommerceOS.PaymentService.Infrastructure.External.StripeGateway;
using ECommerceOS.PaymentService.Infrastructure.Idempotency;
using ECommerceOS.PaymentService.Infrastructure.Background;
using ECommerceOS.PaymentService.Infrastructure.EmailService;
using ECommerceOS.PaymentService.Infrastructure.ServiceCollectionExtensions;
using ECommerceOS.Shared.Auth;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI.Services;
using Quartz;

namespace ECommerceOS.PaymentService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureOptions<StripeSettingsSetup>();
        builder.Services.ConfigureOptions<StripeClientOptionsSetup>();

        builder.Services.AddSingleton<IStripeClient>(static provider =>
        {
            var options = provider.GetRequiredService<IOptions<StripeClientOptions>>().Value;
            var stripeClient = new StripeClient(options);
            return stripeClient;
        });
        
        builder.Services.ConfigureOptions<JwtOptionsSetup>();
        builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        builder.AddPersistence();
        builder.Services.AddAvroSerialization();
        
        builder.AddKafkaProducer<string, IIntegrationEvent>("kafka", (context, producerConfig) =>
        {
            var serializerConfig = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true,
                SubjectNameStrategy = SubjectNameStrategy.Record
            };
            
            var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
            producerConfig.SetValueSerializer(new AvroSerializer<IIntegrationEvent>(schemaRegistryClient, serializerConfig));
        });

        builder.AddKafkaProducer<string, PaymentEvent>("kafka", (context, producerConfig) =>
        {
            var serializerConfig = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true,
                SubjectNameStrategy = SubjectNameStrategy.Record
            };
            
            var schemaRegistryClient = context.GetRequiredService<ISchemaRegistryClient>();
            producerConfig.SetValueSerializer(new AvroSerializer<PaymentEvent>(schemaRegistryClient, serializerConfig));
        });
        
        builder.Services.AddMessaging();
        
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        
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
        
        builder.Services.AddScoped<IPaymentService, StripePaymentService>();
        builder.Services.AddScoped<ITransactionService, StripeTransactionService>();
        builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        
        builder.Services.AddOptions<SmtpClientOptions>()
            .Bind(builder.Configuration.GetSection(nameof(SmtpClientOptions)));

        builder.Services.AddScoped<StripeProcessor>();
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<RefundService>();
        builder.Services.AddScoped<PaymentIntentService>();
        builder.Services.AddScoped<SetupIntentService>();
        builder.Services.AddScoped<CustomerService>();
        builder.Services.AddScoped<PaymentMethodService>();
        builder.Services.AddScoped<StripePaymentService>();
        builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
        
        builder.AddRedisDistributedCache("redis");
        builder.Services.AddFusionCache()
            .WithStackExchangeRedisBackplane();
    }
}
