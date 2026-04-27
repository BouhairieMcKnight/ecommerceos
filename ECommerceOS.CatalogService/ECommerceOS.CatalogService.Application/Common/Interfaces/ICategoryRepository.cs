using ECommerceOS.CatalogService.Application.Common.Models;

namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<Result<Category>> GetCategoryByNameAsync(string? categoryName, CancellationToken cancellationToken = default);
    Task<bool> VerifyCategoryNameAsync(string categoryName, CancellationToken cancellationToken = default);
    Task<bool> VerifyCategoryParentIdAsync(CategoryId parentId, CancellationToken cancellationToken = default);
    Task<bool> VerifyCategoryParentNameAsync(string parentName, CancellationToken cancellationToken = default);
    Task<Result<(Category Category, CategoryId ParentId)>> GetCategoryTreeAsync(
        string categoryName, string parentCategoryName, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CategoryHierarchyNode>>> GetCategoryHierarchyAsync(
        CancellationToken cancellationToken = default);
}
