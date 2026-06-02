using System.Linq.Expressions;

namespace ECommerceOS.OrderService.Infrastructure.Persistence.Data.Repositories;

public class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<Result<Order>> GetByIdAsync(OrderId id, CancellationToken ct = new CancellationToken())
    {
        var order = await dbContext.Set<Order>().FirstOrDefaultAsync(o =>  o.Id == id, ct);
        
        return order is null ? Result<Order>.Failure(OrderErrors.NotFound) : Result<Order>.Success(order);
    }

    public async Task AddAsync(Order entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<Order>().AddAsync(entity, token);
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Order>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task UpdateAsync(Order entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Order entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            dbContext.Set<Order>().Remove(entity); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Order>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task<bool> VerifyOrderAsync(OrderId orderId, UserId userId, CancellationToken ct = new CancellationToken())
    {
        return await dbContext.Set<Order>()
            .AnyAsync(o => o.Id == orderId && o.CustomerId == userId, ct);
    }

    public async Task<bool> VerifyUserOrderAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Order>()
            .AnyAsync(o => o.CustomerId == userId, cancellationToken);
    }

    public Task<Result<IQueryable<Order>>> GetOrdersPaginatedAsync(
        Expression<Func<Order, object>> columnSelector, 
        UserId userId,
        string? sortOrder = "",
        DateTimeOffset? orderDate = null,
        TimeSpan? timeSpan = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> ordersQuery = dbContext.Set<Order>()
            .AsNoTracking()
            .Where(o => o.CustomerId == userId)
            .Include(o => o.OrderItems);
        
        if (orderDate.HasValue && timeSpan.HasValue)
        {
            ordersQuery = ordersQuery
                .Where(o =>
                    o.CreatedOn >= orderDate.Value &&
                    o.CreatedOn <= orderDate.Value + timeSpan.Value); 
        }

        if (!string.IsNullOrEmpty(sortOrder) && sortOrder?.ToLower() == "desc")
        {
            ordersQuery = ordersQuery.OrderByDescending(columnSelector);
        }
        else
        {
            ordersQuery = ordersQuery.OrderBy(columnSelector);
        }

        return Task.FromResult(Result<IQueryable<Order>>.Success(ordersQuery));
    }
}
