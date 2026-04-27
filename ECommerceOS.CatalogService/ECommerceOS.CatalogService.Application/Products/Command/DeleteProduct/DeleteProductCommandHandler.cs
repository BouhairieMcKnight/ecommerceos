namespace ECommerceOS.CatalogService.Application.Products.Command.DeleteProduct;

public class DeleteProductCommandHandler(ICatalogRepository catalogRepository)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = await catalogRepository.GetByIdAsync(request.ProductId!, cancellationToken)
            .Bind(p => p.DeleteProduct())
            .TapAsync(async p => await catalogRepository.DeleteAsync(p, cancellationToken));
        
        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }
}