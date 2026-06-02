namespace ECommerceOS.CatalogService.Application.Carts.Commands.OrderCart;

public record OrderCartCommand(
    OrderId OrderId,
    CartId CartId);