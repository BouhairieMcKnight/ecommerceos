using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Application.Carts.Queries.GetCart;

public record GetCartQuery(UserId UserId) : IQuery<Cart>;