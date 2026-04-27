namespace ECommerceOS.CatalogService.Application.Common.Models;

public sealed record CategoryHierarchyNode(
    CategoryId CategoryId,
    string Title,
    IReadOnlyCollection<CategoryHierarchyNode> Children);
