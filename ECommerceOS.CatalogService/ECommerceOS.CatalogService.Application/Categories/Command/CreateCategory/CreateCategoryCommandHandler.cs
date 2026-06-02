namespace ECommerceOS.CatalogService.Application.Categories.Command.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, CreateCategoryCommandResponse>
{
    public async Task<Result<CreateCategoryCommandResponse>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var result = await Category.Create(request.Title, request.ParentCategoryId)
            .TapAsync(async c => await categoryRepository.AddAsync(c, cancellationToken));
            
        return result.Match(
            success => Result<CreateCategoryCommandResponse>
                    .Success(CreateCategoryCommandResponse.Create(success.Id, success.Title)),
                Result<CreateCategoryCommandResponse>.Failure);
    }
}