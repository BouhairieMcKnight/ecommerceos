using ECommerceOS.Shared.Contracts.Messaging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ECommerceOS.AuthService.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<OutboxMessage> OutBoxMessages { get; set; }
    public DatabaseFacade DatabaseFacade { get; set; }

    static IdentityDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}