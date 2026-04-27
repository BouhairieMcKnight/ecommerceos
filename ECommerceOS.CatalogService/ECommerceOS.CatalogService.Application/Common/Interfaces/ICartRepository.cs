using ECommerceOS.CatalogService.Domain.Carts;
using ECommerceOS.CheckoutService;

namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICartRepository : IRepository<Cart, CartId>
{
    Task<bool> VerifyUserAsync(UserId userId, CancellationToken cancellationToken = default);
    
    Task<Result<Cart>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    
    Task<Result<List<CheckoutRequestModel>>> GetCheckoutRequestAsync(UserId customerId,
        CancellationToken cancellationToken = default);
    
}
