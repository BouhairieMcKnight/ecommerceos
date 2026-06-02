namespace ECommerceOS.CatalogService.Application.Categories.Command.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (title, ct) =>
                await categoryRepository.VerifyCategoryNameAsync(title, ct))
            .WithMessage("Title field is required");
    }
}