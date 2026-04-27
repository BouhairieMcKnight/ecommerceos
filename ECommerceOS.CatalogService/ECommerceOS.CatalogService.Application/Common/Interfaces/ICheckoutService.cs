using ECommerceOS.CheckoutService;

namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICheckoutCartService
{
    Task<Result<string>> StartCheckoutAsync(UserId customerId, List<CheckoutRequestModel> items,
        CancellationToken cancellationToken = new());
}