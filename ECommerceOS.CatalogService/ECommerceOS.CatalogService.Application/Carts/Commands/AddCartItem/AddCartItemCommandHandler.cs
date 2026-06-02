using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Application.Carts.Commands.AddCartItem;

public class AddCartItemCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<AddCartItemCommand, Cart>
{
    public async Task<Result<Cart>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            return Result<Cart>.Failure(Error.Validation("Cart.UserId", "User Id is required"));
        }

        var cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (!cart.IsSuccess)
        {
            var created = Cart.Create(request.UserId);
            var inserted = await created.Bind(c => c.AddItem(
                    request.ProductId,
                    request.Price,
                    request.ImageUrl,
                    request.Quantity,
                    request.Description,
                    request.Name))
                .TapAsync(async c => await cartRepository.AddAsync(c, cancellationToken));

            return inserted.Match(
                Result<Cart>.Success,
                Result<Cart>.Failure);
        }

        var result = await cart.Bind(c => c.AddItem(
                request.ProductId,
                request.Price,
                request.ImageUrl,
                request.Quantity,
                request.Description,
                request.Name))
            .TapAsync(async c => await cartRepository.UpdateAsync(c, cancellationToken));

        return result.Match(
            Result<Cart>.Success,
            Result<Cart>.Failure);
    }
}
