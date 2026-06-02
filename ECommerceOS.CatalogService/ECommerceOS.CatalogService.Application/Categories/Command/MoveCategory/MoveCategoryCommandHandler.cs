namespace ECommerceOS.CatalogService.Application.Categories.Command.MoveCategory;

public class MoveCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<MoveCategoryCommand>
{
    public async Task<Result> Handle(MoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var result= await categoryRepository.GetCategoryTreeAsync(
            request.CategoryName, request.ParentCategoryName, cancellationToken)
            .Bind(o => o.Category.MoveTo(o.ParentId))
            .TapAsync(c => categoryRepository.UpdateAsync(c, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}