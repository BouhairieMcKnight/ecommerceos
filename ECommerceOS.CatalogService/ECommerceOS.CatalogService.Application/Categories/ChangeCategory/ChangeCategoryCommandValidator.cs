namespace ECommerceOS.CatalogService.Application.Categories.ChangeCategory;

public class ChangeCategoryCommandValidator : AbstractValidator<ChangeCategoryCommand>
{
    public ChangeCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(command => command.CategoryTitle)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (title, ct) => await categoryRepository.VerifyCategoryNameAsync(title, ct))
            .WithMessage("Category title cannot be empty and must already exist");
        
        RuleFor(command => command.NewCategoryTitle)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (newTitle, ct) =>
                await categoryRepository.VerifyCategoryNameAsync(newTitle, ct))
            .WithMessage("Ne category title cannot be empty and must not already exist");
    }
}