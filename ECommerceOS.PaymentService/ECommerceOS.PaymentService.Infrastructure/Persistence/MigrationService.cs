namespace ECommerceOS.PaymentService.Infrastructure.Persistence;

internal sealed class MigrationService(
    IServiceProvider serviceProvider,
    ILogger<MigrationService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying database migrations...");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error occured during migration");
        }
        
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Database migrations applied.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}