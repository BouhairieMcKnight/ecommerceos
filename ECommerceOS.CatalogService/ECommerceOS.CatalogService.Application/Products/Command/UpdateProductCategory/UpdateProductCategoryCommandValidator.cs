namespace ECommerceOS.CatalogService.Application.Products.Command.UpdateProductCategory;

public class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
{
    public UpdateProductCategoryCommandValidator(ICatalogRepository catalogRepository)
    {
        RuleFor(command => command.Sku)
            .NotNull()
            .NotEmpty()
            .MustAsync(async (sku, ct) => 
                await catalogRepository.VerifySkuAsync(sku, ct))
            .WithMessage("Sku doesn't exist");
    }
}