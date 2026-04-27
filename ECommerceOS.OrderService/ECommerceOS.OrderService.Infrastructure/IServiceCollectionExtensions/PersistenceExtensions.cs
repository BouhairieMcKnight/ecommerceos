using ECommerceOS.OrderService.Infrastructure.Persistence.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerceOS.OrderService.Infrastructure.IServiceCollectionExtensions;

public static class PersistenceExtensions
{
    public static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.AddNpgsqlDataSource("orderdb");
        
        builder.Services.AddDbContext<OrderDbContext>((sp, options) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("orderdb");
            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(typeof(OrderDbContext).Assembly);
                optionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "order");
            });
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<OrderDbContext>();
        
        builder.Services.AddHostedService<MigrationService>();
        
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    }
}