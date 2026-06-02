namespace ECommerceOS.CatalogService.Application.Categories.Command.MoveCategory;

public record MoveCategoryCommand(string CategoryName, string ParentCategoryName) : ICommand;