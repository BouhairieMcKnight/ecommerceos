namespace ECommerceOS.CatalogService.Application.Carts.Commands.CheckoutCart;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCartCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(c => c.CustomerId)
            .NotEmpty()
            .NotNull()
            .WithMessage("CustomerId is required");
    }
}