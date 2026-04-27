using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerceOS.CatalogService.Infrastructure.IServiceCollectionExtensions;

public static class PersistenceExtensions
{
    public static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        
        builder.AddNpgsqlDataSource("catalogdb");

        builder.Services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("catalogdb"); 
            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(typeof(CatalogDbContext).Assembly);
                optionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "catalog");
            });
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<CatalogDbContext>();

        builder.Services.AddHostedService<MigrationService>();
        
        builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();
    }
}
