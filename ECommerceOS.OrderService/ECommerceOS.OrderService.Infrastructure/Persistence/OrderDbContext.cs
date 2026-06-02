using ECommerceOS.OrderService.Infrastructure.Persistence.Data.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ECommerceOS.OrderService.Infrastructure.Persistence;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options) 
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxMessage> OutBoxMessages { get; set; }
    public DatabaseFacade DatabaseFacade { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}