namespace ECommerceOS.CatalogService.Application.Categories.Command.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (title, ct) => 
                !(await categoryRepository.VerifyCategoryNameAsync(title, ct)))
            .WithMessage("Title is and must be unique required");

        RuleFor(command => command.ParentCategoryId)
            .MustAsync(async (parentId, ct) => 
                await categoryRepository.VerifyCategoryParentIdAsync(parentId!, ct))
            .When(command => command.ParentCategoryId != null)
            .WithMessage("Parent category does not exist");
    }
}