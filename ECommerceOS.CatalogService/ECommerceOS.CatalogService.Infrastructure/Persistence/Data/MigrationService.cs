namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data;

internal sealed class MigrationService(
    IServiceProvider serviceProvider,
    ILogger<MigrationService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying database migrations...");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error occured during migration");
        }
        
        var sku = Sku.Create("SKU000000000001") ?? throw new InvalidOperationException("Invalid seed SKU.");
        var price = Money.Create(Currency.Usd, 29.99m) ?? throw new InvalidOperationException("Invalid seed money.");
        var productResult = Product.Create(
            sellerId: new UserId(Guid.NewGuid()),
            price: price,
            sku: sku,
            name: "Seed Product",
            description: "Default seeded product for local development.",
            quantity: 100,
            productId: new ProductId(Guid.NewGuid()));

        await context.Products.AddAsync(productResult.Value!, cancellationToken).ConfigureAwait(false);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Database migrations applied.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}