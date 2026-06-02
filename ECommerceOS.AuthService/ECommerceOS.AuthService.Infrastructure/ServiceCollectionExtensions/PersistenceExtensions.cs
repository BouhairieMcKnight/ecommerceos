using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerceOS.AuthService.Infrastructure.ServiceCollectionExtensions;

public static class PersistenceExtensions
{
    public static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.AddNpgsqlDataSource("identitydb");
        
        builder.Services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("identitydb");
            options.UseNpgsql(connectionString, opts =>
            {
                opts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<IdentityDbContext>();

        builder.Services.AddHostedService<MigrationService>();
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }
}