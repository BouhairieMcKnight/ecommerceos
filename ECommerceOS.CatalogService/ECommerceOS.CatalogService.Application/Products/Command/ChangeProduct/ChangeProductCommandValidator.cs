namespace ECommerceOS.CatalogService.Application.Products.Command.ChangeProduct;

public class ChangeProductCommandValidator : AbstractValidator<ChangeProductCommand>
{
    public  ChangeProductCommandValidator(ICatalogRepository catalogRepository)
    {
        RuleFor(command => command.ProductId)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (productId, ct) => 
                await catalogRepository.VerifyProductAsync(productId, ct))
            .WithMessage("Product Id is invalid");

        RuleFor(command => command.Price)
            .NotEmpty()
            .Must(price => price!.Amount > 0 && price.CurrencyValue.Code is "EUR" or "US")
            .WithMessage("Price is invalid")
            .When(command => command.Price is not null);
        
        RuleFor(command => command.Description)
            .NotEmpty()
            .MinimumLength(50)
            .MaximumLength(150)
            .WithMessage("Description is invalid")
            .When(command => command.Description is not null);
    }
}