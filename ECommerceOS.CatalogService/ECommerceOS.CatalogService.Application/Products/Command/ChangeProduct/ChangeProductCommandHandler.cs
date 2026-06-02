namespace ECommerceOS.CatalogService.Application.Products.Command.ChangeProduct;

public class ChangeProductCommandHandler(ICatalogRepository catalogRepository) 
    : ICommandHandler<ChangeProductCommand, ChangeProductCommandResponse> 
{
    public async Task<Result<ChangeProductCommandResponse>> Handle(
        ChangeProductCommand request,
        CancellationToken cancellationToken)
    {
        var productResult = await catalogRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess)
        {
            return Result<ChangeProductCommandResponse>.Failure(productResult.Error!);
        }

        var product = productResult.Value!;
        
        if (request.Price is not null)
        {
            var updatePriceResult = product.UpdatePrice(request.Price);
            if (!updatePriceResult.IsSuccess)
            {
                return Result<ChangeProductCommandResponse>.Failure(updatePriceResult.Error!);
            }
        }

        if (request.Description is not null)
        {
            var updateDescriptionResult = product.UpdateDescription(request.Description);
            if (!updateDescriptionResult.IsSuccess)
            {
                return Result<ChangeProductCommandResponse>.Failure(updateDescriptionResult.Error!);
            }
        }

        await catalogRepository.UpdateAsync(product, cancellationToken);

        return Result<ChangeProductCommandResponse>.Success(new ChangeProductCommandResponse(product.Id));
    }
}
