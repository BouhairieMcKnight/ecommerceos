using ECommerceOS.CatalogService.Presentation.Product.UploadImages;

namespace ECommerceOS.CatalogService.Presentation.Product;

public static class ProductExtensions
{
    private const string RoutePrefix = "/products";
    
    public static RouteGroupBuilder MapProductsGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapDeleteByIdEndpoint();
        group.MapPutImagesEndpoint();
        group.MapProductGetByIdEndpoint();
        group.MapProductPostEndpoint();
        group.MapGetPaginatedProductsEndpoint();

        return group;
    }
}
