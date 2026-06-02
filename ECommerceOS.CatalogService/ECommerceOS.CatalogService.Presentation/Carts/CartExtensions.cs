using ECommerceOS.CatalogService.Presentation.Carts.GetCart;
using ECommerceOS.CatalogService.Presentation.Carts.Post;
using ECommerceOS.CatalogService.Presentation.Carts.Checkouts;

namespace ECommerceOS.CatalogService.Presentation.Carts;

public static class CartExtensions
{
    private const string RoutePrefix = "/cart";

    public static RouteGroupBuilder MapCartGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapGetCartEndpoint();
        group.MapPostCartItemEndpoint();
        group.MapPostCheckoutEndpoint();

        return group;
    }
}
