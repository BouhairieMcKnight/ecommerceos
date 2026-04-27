using ECommerceOS.CatalogService.Domain.Carts;
using ECommerceOS.CheckoutService;
using Error = ECommerceOS.Shared.Result.Error;

namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Repositories;

public class CartRepository(CatalogDbContext dbContext) : ICartRepository
{
    public async Task<Result<Cart>> GetByIdAsync(CartId id, CancellationToken ct = new CancellationToken())
    {
        var result = await dbContext.Set<Cart>()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        
        return result is null ? 
            Result<Cart>.Failure(Error.NotFound("Cart", "Cart Not Found")) :
            Result<Cart>.Success(result);
    }

    public async Task AddAsync(Cart entity, CancellationToken ct = new CancellationToken())
    {
        var execution = dbContext.Database.CreateExecutionStrategy();
        
        await execution.ExecuteInTransactionAsync(async (token) =>
            {
                await dbContext.Carts.AddAsync(entity, token);
                await dbContext.SaveChangesAsync(token);
            },
            async (token) =>
            {
                return await dbContext.Carts.AnyAsync(c => c.Id == entity.Id, token);
            }, ct);
    }

    public async Task UpdateAsync(Cart entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Cart entity, CancellationToken ct = new CancellationToken())
    {
        var execution = dbContext.Database.CreateExecutionStrategy();
        
        await execution.ExecuteInTransactionAsync(async (token) =>
            {
                dbContext.Carts.Remove(entity);
                await dbContext.SaveChangesAsync(token);
            },
            async (token) =>
            {
                return !(await dbContext.Carts.AnyAsync(c => c.Id == entity.Id, token));
            }, ct);
    }

    public async Task<bool> VerifyUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Cart>()
            .AnyAsync(c => c.CustomerId == userId, cancellationToken);
    }

    public async Task<Result<Cart>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<Cart>()
            .FirstOrDefaultAsync(c => c.CustomerId == userId, cancellationToken: cancellationToken);
        
        return result is null ?
            Result<Cart>.Failure(Error.NotFound("Cart", "Cart Not Found")) :
            Result<Cart>.Success(result);
    }

    public async Task<Result<List<CheckoutRequestModel>>> GetCheckoutRequestAsync(
        UserId customerId, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Carts
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .SelectMany(c => c.CartItems)
            .Join(
                dbContext.Products.AsNoTracking(),
                cartItem => cartItem.ProductId,
                product => product.Id,
                (cartItem, product) => new CheckoutRequestModel()
                {
                    SellerId = product.SellerId.Value.ToString(),
                    ProductId = product.Id.Value.ToString(),
                    ImageUrl = cartItem.ImageUrl,
                    Description = cartItem.Description,
                    Quantity = cartItem.Quantity,
                    Price = new MoneyModel()
                    {
                        Amount = (float)cartItem.Price.Amount, 
                        Currency = cartItem.Price.CurrencyValue.Code
                    },
                    Name = cartItem.Name
                })
            .ToListAsync(cancellationToken);
        
        return result.Any() ?
            Result<List<CheckoutRequestModel>>.Success(result) :
            Result<List<CheckoutRequestModel>>.Failure(Error.NotFound("Cart", "Cart Not Found"));
    }
}
