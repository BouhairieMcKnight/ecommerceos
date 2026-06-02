using ECommerceOS.OrderService.Application.Orders.Query.GetOrder;
using ECommerceOS.OrderService.Presentation.Http;
using ECommerceOS.Shared.Result;
using ECommerceOS.Shared.ValueObjects;

namespace ECommerceOS.OrderService.Presentation.Order.GetById;

public static class Endpoint
{
    private const string Route = "/{id}";

    public static RouteHandlerBuilder MapGetByIdEndpoint(this RouteGroupBuilder groupBuilder)
    {
        return groupBuilder.MapGet(Route, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var orderId = new OrderId(Guid.Parse(id));
        var userId = httpContext.GetUserId();
        var query = new GetOrderQuery(orderId, userId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
    }
}