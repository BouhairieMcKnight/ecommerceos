namespace ECommerceOS.CatalogService.Application.Categories.Command.MoveCategory;

public class MoveCategoryCommandValidator : AbstractValidator<MoveCategoryCommand>
{
    public MoveCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(command => command.CategoryName)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (categoryName, cancellationToken) => 
                await categoryRepository.VerifyCategoryNameAsync(categoryName, cancellationToken))
            .WithMessage("Category name cannot be empty and must exist");
        
        RuleFor(command => command.ParentCategoryName)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (parentName, cancellationToken) => 
                await categoryRepository.VerifyCategoryParentNameAsync(parentName, cancellationToken))
            .WithMessage("Parent category name cannot be empty and must exist outside of current category");
    }
}