namespace ECommerceOS.CatalogService.Application.Categories.ChangeCategory;

public record ChangeCategoryCommand(string CategoryTitle, string NewCategoryTitle) : ICommand;