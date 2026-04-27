namespace ECommerceOS.OrderService.Application.Orders.Query.GetOrder;

public record GetOrderQuery(OrderId? OrderId, UserId? UserId) 
    : IQuery<GetOrderQueryResponse>, ICachedQuery
{
    public string Tag => nameof(Order);
    public TimeSpan CacheDuration { get; } = TimeSpan.FromHours(10);
    public string CacheKey => OrderId.Value.ToString("D");
}

public record GetOrderQueryResponse(OrderId OrderId);