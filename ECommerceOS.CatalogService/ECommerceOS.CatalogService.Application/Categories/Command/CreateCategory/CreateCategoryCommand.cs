namespace ECommerceOS.CatalogService.Application.Categories.Command.CreateCategory;

public record CreateCategoryCommand(
    string Title,
    CategoryId? ParentCategoryId = null) 
    : ICommand<CreateCategoryCommandResponse>;

public record CreateCategoryCommandResponse(CategoryId CategoryId, string Title)
{
    public static CreateCategoryCommandResponse Create(CategoryId categoryId, string title)
    {
        return new(categoryId, title);
    }
};