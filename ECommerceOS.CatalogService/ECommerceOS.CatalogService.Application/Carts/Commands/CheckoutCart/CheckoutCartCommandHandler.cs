using ECommerceOS.CheckoutService;

namespace ECommerceOS.CatalogService.Application.Carts.Commands.CheckoutCart;

public class CheckoutCartCommandHandler(
    ICheckoutCartService checkoutService,
    ICartRepository cartRepository)
    : ICommandHandler<CheckoutCartCommand, string>
{
    public async Task<Result<string>> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        var checkoutRequest = await cartRepository.GetCheckoutRequestAsync(request.CustomerId, cancellationToken);
        var result = await checkoutRequest
            .BindAsync(async r =>
                await checkoutService.StartCheckoutAsync(request.CustomerId, r, cancellationToken));

        return result.Match(
            Result<string>.Success,
            Result<string>.Failure);
    }
}
