namespace ECommerceOS.CatalogService.Application.Products.Query.GetProduct;

public class GetProductQueryHandler(
    ICatalogRepository catalogRepository)
    : IQueryHandler<GetProductQuery, GetProductResponse>
{
    public async Task<Result<GetProductResponse>> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var product = await catalogRepository.GetByIdAsync(request.ProductId, cancellationToken);

        return product.Match(
            success => Result<GetProductResponse>.Success(new GetProductResponse(
                success.Id,
                success.Name,
                success.Description,
                success.Price,
                success.Quantity,
                success.ImageUrls.ToArray())),
            Result<GetProductResponse>.Failure);
    }
}
