namespace ECommerceOS.CatalogService.Application.Categories.Command.DeleteCategory;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = await categoryRepository.GetCategoryByNameAsync(request.Title, cancellationToken)
            .Bind(c => c.Delete())
            .TapAsync(async c => await categoryRepository.DeleteAsync(c, cancellationToken));

        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}