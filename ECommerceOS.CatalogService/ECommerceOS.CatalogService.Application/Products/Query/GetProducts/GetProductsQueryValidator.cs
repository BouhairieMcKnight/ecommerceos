namespace ECommerceOS.CatalogService.Application.Products.Query.GetProducts;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(query => query.SortOrder)
            .NotEmpty()
            .Must(sort => sort!.Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                || sort.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("Sort order can only be ascending or descending")
            .When(query => query.SortOrder is not null);

        RuleFor(query => query.SearchCategory)
            .NotEmpty()
            .MustAsync(async (category, cancellationToken) => 
                await categoryRepository.VerifyCategoryNameAsync(category!, cancellationToken))
            .WithMessage("Search category name is invalid")
            .When(query => query.SearchCategory is not null);
        
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1");
        
        RuleFor(query => query.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than or equal to 1");
    }
}