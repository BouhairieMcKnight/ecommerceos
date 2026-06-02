namespace ECommerceOS.CatalogService.Application.Products.Query.GetProducts;

public class GetProductsQueryHandler(
    ICatalogRepository catalogRepository,
    ICategoryRepository categoryRepository)
    : IQueryHandler<GetProductsQuery, PaginatedList<GetProductsQueryResponse>>
{
    public async Task<Result<PaginatedList<GetProductsQueryResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var categoryResult = await categoryRepository.GetCategoryByNameAsync(request.SearchCategory,
            cancellationToken);
        
        var categoryId = categoryResult.IsSuccess ? categoryResult.Value!.Id : null;
        var column = ColumnSortSelector(request);
        
        var queryable = await catalogRepository.GetProductsPaginatedAsync(
            columnSelector: column,
            searchCategory: categoryId,
            sortOrder: request.SortOrder,
            searchTerm: request.SearchTerm,
            cancellationToken);
        
        var result = await queryable.BindAsync(async query =>
        {
            var res = await query.ProjectToType<GetProductsQueryResponse>()
                .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
            
            return Result<PaginatedList<GetProductsQueryResponse>>.Success(res);
        });

        return result;
    }

    private static Expression<Func<Product, object>> ColumnSortSelector(GetProductsQuery request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name,
            "sku" => product => product.Sku,
            "price" => product => product.Price.Amount,
            _ => product => product.Id
        };
    }
}