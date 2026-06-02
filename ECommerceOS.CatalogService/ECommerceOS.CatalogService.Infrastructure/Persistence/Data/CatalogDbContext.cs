using ECommerceOS.CatalogService.Domain.Carts;
using ECommerceOS.Shared.Contracts.Messaging.Catalog;

namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryClosure> CategoryClosures { get; set; }
    public DbSet<Cart> Carts { get; set; }

    static CatalogDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
