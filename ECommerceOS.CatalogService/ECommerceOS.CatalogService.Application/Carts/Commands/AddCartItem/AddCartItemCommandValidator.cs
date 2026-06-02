namespace ECommerceOS.CatalogService.Application.Carts.Commands.AddCartItem;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator(ICartRepository cartRepository, ICartService cartService)
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (id, ct) => await cartRepository.VerifyUserAsync(id, ct))
            .When(id => id is not null)
            .WithMessage("User not found");
        
        RuleFor(c => c.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("User Id cannot be null");

        RuleFor(c => c)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (c, ct) => 
                await cartService.CheckProductStock(c.ProductId, c.Quantity, ct))
            .WithMessage("Product amount is not available");
    }
}