namespace ECommerceOS.CatalogService.Application.Products.Command.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(ISkuService skuService)
    {
        RuleFor(command => command.Description)
            .NotEmpty()
            .WithMessage("Description is required");
        
        RuleFor(command => command.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");

        RuleFor(command => command.Price)
            .NotEmpty()
            .Must(price => price.Amount > 0)
            .WithMessage("Price must be greater than 0");
        
        RuleFor(command => command.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(command => command.SellerId)
            .NotNull()
            .WithMessage("Seller is required");
    }
}
