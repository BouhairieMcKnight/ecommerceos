using ECommerceOS.CatalogService.Application.Common.Models;

namespace ECommerceOS.CatalogService.Application.Categories.Query.GetCategoryHierarchy;

public record GetCategoryHierarchyQuery : IQuery<IReadOnlyCollection<CategoryHierarchyNode>>;
