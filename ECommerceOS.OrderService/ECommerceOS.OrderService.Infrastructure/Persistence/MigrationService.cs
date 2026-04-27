namespace ECommerceOS.OrderService.Infrastructure.Persistence;

internal sealed class MigrationService(
    IServiceProvider serviceProvider,
    ILogger<MigrationService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying database migrations...");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error occured during migration");
        }

        /*var userResult = User.Create(
            userName: "Seed Customer",
            role: Role.Customer,
            email: "mcknight.bhairie@gmail.com",
            password: "seed-password",
            customerId: new UserId(Guid.NewGuid()));

        if (!userResult.IsSuccess || userResult.Value is null)
        {
            throw new InvalidOperationException("Failed to create auth seed user.");
        }

        await context.Users.AddAsync(userResult.Value, cancellationToken).ConfigureAwait(false)*/;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Database migrations applied.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}