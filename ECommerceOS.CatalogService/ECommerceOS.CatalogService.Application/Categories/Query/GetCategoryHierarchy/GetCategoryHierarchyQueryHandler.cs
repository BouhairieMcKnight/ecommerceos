using ECommerceOS.CatalogService.Application.Common.Models;

namespace ECommerceOS.CatalogService.Application.Categories.Query.GetCategoryHierarchy;

public class GetCategoryHierarchyQueryHandler(
    ICategoryRepository categoryRepository) : IQueryHandler<GetCategoryHierarchyQuery, IReadOnlyCollection<CategoryHierarchyNode>>
{
    public async Task<Result<IReadOnlyCollection<CategoryHierarchyNode>>> Handle(
        GetCategoryHierarchyQuery request,
        CancellationToken cancellationToken)
    {
        return await categoryRepository.GetCategoryHierarchyAsync(cancellationToken);
    }
}
