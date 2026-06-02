namespace ECommerceOS.CatalogService.Application.Carts.Commands.CheckoutCart;

public record CheckoutCartCommand(Guid IdempotentCommandId, UserId CustomerId)
    : ICommand<string>, IIdempotentCommand;