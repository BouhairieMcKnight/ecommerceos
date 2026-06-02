using System.Linq.Expressions;
using Mapster;

namespace ECommerceOS.OrderService.Application.Orders.Query.GetPaginatedOrders;

public class GetPaginatedOrdersQueryHandler(
    IOrderRepository orderRepository)
    : IQueryHandler<GetPaginatedOrdersQuery, PaginatedList<GetPaginatedOrdersQueryResponse>>
{
    public async Task<Result<PaginatedList<GetPaginatedOrdersQueryResponse>>> Handle(
        GetPaginatedOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var column = ColumnSortSelector(request);
        
        var queryable = await orderRepository.GetOrdersPaginatedAsync(
            columnSelector: column,
            userId: request.UserId!,
            sortOrder: request.SortOrder,
            orderDate: request.OrderDate,
            timeSpan: request.Range,
            cancellationToken);
        
        var result = await queryable.BindAsync(async query =>
        {
            var res = await query
                .ProjectToType<GetPaginatedOrdersQueryResponse>()
                .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
            
            return Result<PaginatedList<GetPaginatedOrdersQueryResponse>>.Success(res);
        });

        return result;
    }
    
    private static Expression<Func<Order, object>> ColumnSortSelector(GetPaginatedOrdersQuery request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "latest" => order => order.CreatedOn,
            "status" => order => order.Status,
            _ => order => order.Id
        };
    }
}