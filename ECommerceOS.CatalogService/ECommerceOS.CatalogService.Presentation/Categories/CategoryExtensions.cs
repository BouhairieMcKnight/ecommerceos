using ECommerceOS.CatalogService.Presentation.Categories.GetHierarchy;

namespace ECommerceOS.CatalogService.Presentation.Categories;

public static class CategoryExtensions
{
    private const string RoutePrefix = "/categories";

    public static RouteGroupBuilder MapCategoriesGroup(this WebApplication app)
    {
        var group = app.MapGroup(RoutePrefix);

        group.MapGetCategoryHierarchyEndpoint();

        return group;
    }
}
