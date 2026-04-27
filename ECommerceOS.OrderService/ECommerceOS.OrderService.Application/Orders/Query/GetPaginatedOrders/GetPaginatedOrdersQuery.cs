namespace ECommerceOS.OrderService.Application.Orders.Query.GetPaginatedOrders;

public record GetPaginatedOrdersQuery(
    UserId? UserId,
    string? SortOrder,
    string? SortColumn,
    DateTimeOffset? OrderDate,
    TimeSpan? Range,
    int PageNumber,
    int PageSize)
    : IQuery<PaginatedList<GetPaginatedOrdersQueryResponse>>, ICachedQuery
{
    public string Tag => nameof(Order);
    public string CacheKey => $"{UserId?.Value}_{SortOrder}_{SortColumn}_{OrderDate}_{Range}_{PageNumber}_{PageSize}";
    public TimeSpan CacheDuration { get; } = TimeSpan.FromHours(15);
}

public record GetPaginatedOrdersQueryResponse(
    OrderId OrderId,
    DateTimeOffset CreatedOn,
    IEnumerable<OrderItem> OrderItems);