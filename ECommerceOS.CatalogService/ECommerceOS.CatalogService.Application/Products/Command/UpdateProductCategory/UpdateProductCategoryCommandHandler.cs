namespace ECommerceOS.CatalogService.Application.Products.Command.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler(
    ICatalogRepository catalogRepository)
    : ICommandHandler<UpdateProductCategoryCommand>
{
    public async Task<Result> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var products = await catalogRepository.GetProductsAsync(request.Sku, cancellationToken);

        var result = await products.Bind(product =>
            {
                var enumerable = product as Product[] ?? product.ToArray();
                var errors = enumerable
                    .Select(p => p.UpdateCategory(request.CategoryId))
                    .Where(result => !result.IsSuccess)
                    .Select(result => result.Error)
                    .Distinct()
                    .ToArray();

                return errors.Any()
                    ? Result<IEnumerable<Product>>.Failure(errors!)
                    : Result<IEnumerable<Product>>.Success(enumerable);
            })
            .TapAsync(p=> catalogRepository.UpdateProductsAsync(p, cancellationToken));
            
        return result 
            .Match(
                success => Result.Success(),
                Result.Failure);
    }
}
