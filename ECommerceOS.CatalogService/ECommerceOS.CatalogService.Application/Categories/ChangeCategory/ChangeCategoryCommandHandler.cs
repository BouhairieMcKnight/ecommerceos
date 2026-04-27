namespace ECommerceOS.CatalogService.Application.Categories.ChangeCategory;

public class ChangeCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<ChangeCategoryCommand>
{
    public async Task<Result> Handle(ChangeCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = await categoryRepository
            .GetCategoryByNameAsync(request.CategoryTitle, cancellationToken)
            .Bind(c => c.Rename(request.NewCategoryTitle))
            .TapAsync(async c => await categoryRepository.UpdateAsync(c, cancellationToken));
        
        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}