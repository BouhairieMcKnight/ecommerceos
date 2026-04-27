using System.Linq.Expressions;

namespace ECommerceOS.OrderService.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    Task<bool> VerifyOrderAsync(OrderId orderId, UserId userId, CancellationToken cancellationToken = default);

    Task<bool> VerifyUserOrderAsync(UserId userId, CancellationToken cancellationToken = default);
    
    Task<Result<IQueryable<Order>>> GetOrdersPaginatedAsync(
        Expression<Func<Order, object>> columnSelector,
        UserId userId,
        string? sortOrder = "",
        DateTimeOffset? orderDate = null,
        TimeSpan? timeSpan = null,
        CancellationToken cancellationToken = default);
}