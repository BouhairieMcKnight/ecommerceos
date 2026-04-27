using ECommerceOS.AuthService.Infrastructure.Security;

namespace ECommerceOS.AuthService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddOptions<SmtpClientOptions>().Bind(builder.Configuration.GetSection(nameof(SmtpClientOptions)));
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddSingleton<Random>(_ => new Random());
        builder.Services.AddAvroRegistrations();
        builder.Services.AddMessaging(builder.Configuration);
        builder.AddKafkaProducer<string, IdentityEvent>("kafka", static (sp, options) =>
        {
            var schemaRegistryClient = sp.GetRequiredService<ISchemaRegistryClient>();
            var config = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true,
                SubjectNameStrategy = SubjectNameStrategy.Record
            };
            
            options.SetValueSerializer(new AvroSerializer<IdentityEvent>(schemaRegistryClient, config));
        });

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
        
        builder.AddPersistence();
        
        builder.Services.ConfigureOptions<JwtOptionsSetup>();
        builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        builder.AddRedisDistributedCache("cache");
        builder.Services.AddFusionCache()
            .WithStackExchangeRedisBackplane();
        builder.Services.AddOptions<KeyOptions>().Bind(builder.Configuration.GetSection(nameof(KeyOptions)));
        builder.Services.AddScoped<IEncryptionService, EncryptionService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.ConfigureOptions<JwtOptionsSetup>();
        builder.Services.ConfigureOptions<GoogleAuthOptionsSetup>();
        builder.Services.ConfigureOptions<GoogleOptionsSetup>();
        builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
        builder.Services.AddTransient<IClaimsTransformation, GoogleClaimTransformation>();
    }
}
