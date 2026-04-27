using ECommerceOS.OrderService.Presentation.Order.DeleteById;
using ECommerceOS.OrderService.Presentation.Order.GetById;
using ECommerceOS.OrderService.Presentation.Order.GetPaginatedOrders;

namespace ECommerceOS.OrderService.Presentation.Order;

public static class OrderExtensions
{
    private const string RoutePrefix = "/orders";

    public static RouteGroupBuilder MapOrderGroupEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapGetByIdEndpoint();
        group.MapPaginatedOrdersEndpoint();
        group.MapDeleteByIdEndpoint();
        return group;
    }

}