using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Application.Carts.Commands.AddCartItem;

public record AddCartItemCommand : ICommand<Cart>, IIdempotentCommand
{
    public UserId? UserId { get; init; }
    public required ProductId ProductId { get; init; }
    public required int Quantity  { get; init; }
    public required string ImageUrl  { get; init; }
    public required string Description { get; init; }
    public required Money Price { get; init; }
    public required string Name { get; init; }
    public Guid IdempotentCommandId { get; init; }
}
