namespace ECommerceOS.CatalogService.Application.Products.Command.ReserveInventory;

public record ReserveInventoryCommand : ICommand, IIdempotentCommand
{
    public Guid IdempotentCommandId { get; init; }
    public required UserId UserId { get; init; }
    public required Dictionary<ProductId, int> ReserveProducts { get; init; }
}