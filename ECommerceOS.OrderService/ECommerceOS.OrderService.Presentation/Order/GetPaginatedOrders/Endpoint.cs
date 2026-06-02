namespace ECommerceOS.OrderService.Presentation.Order.GetPaginatedOrders;

public static class Endpoint
{
    private const string Route = "/";

    public static RouteHandlerBuilder MapPaginatedOrdersEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [AsParameters] PaginatedOrders paginatedOrders,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContext.GetUserId();

        var query = new GetPaginatedOrdersQuery(
            UserId: userId,
            OrderDate: paginatedOrders.OrderDate,
            Range: paginatedOrders.Range,
            SortColumn: paginatedOrders.SortColumn,
            PageNumber: paginatedOrders.PageNumber,
            PageSize: paginatedOrders.PageSize,
            SortOrder: paginatedOrders.SortOrder);
        
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value!) : result.ToProblemDetails();
    }
}

public record struct PaginatedOrders(
    string? SortOrder,
    string? SortColumn,
    int PageNumber,
    int PageSize,
    DateTimeOffset? OrderDate = null,
    TimeSpan? Range = null);