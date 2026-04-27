using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler(
    ICartRepository cartRepository) : IQueryHandler<GetCartQuery, Cart>
{
    public async Task<Result<Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return cart;
    }
}