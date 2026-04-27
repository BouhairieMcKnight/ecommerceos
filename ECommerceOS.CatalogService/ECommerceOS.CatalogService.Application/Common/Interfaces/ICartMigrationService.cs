namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICartMigrationService
{
    Task MigrateSessionToCart(UserId customerId, CancellationToken cancellationToken = new());
}