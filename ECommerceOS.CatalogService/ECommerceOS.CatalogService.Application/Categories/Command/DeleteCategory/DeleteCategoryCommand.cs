namespace ECommerceOS.CatalogService.Application.Categories.Command.DeleteCategory;

public record DeleteCategoryCommand(string Title) : ICommand;