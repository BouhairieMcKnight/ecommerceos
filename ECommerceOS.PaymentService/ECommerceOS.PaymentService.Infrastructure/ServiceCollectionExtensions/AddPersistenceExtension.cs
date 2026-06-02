using System.Reflection;
using ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerceOS.PaymentService.Infrastructure.ServiceCollectionExtensions;

public static class AddPersistenceExtension
{
    public static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.AddNpgsqlDataSource("paymentdb");
        
        builder.Services.AddDbContext<PaymentDbContext>((sp, options) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("paymentdb");
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<PaymentDbContext>();
        
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddHostedService<MigrationService>();
    }
}
