namespace ECommerceOS.CatalogService.Domain.Categories;

public static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound("Category.NotFound", "Category not found");
    
    public static readonly Error NotValidCategory = Error.Validation("Category.Validation",
        "Category is not valid");

    public static Error NotFoundByTitle(string title) => Error.NotFound("Category.NotFoundByTitle",
        $"No category found associated with {title}");
}