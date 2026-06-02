namespace ECommerceOS.CatalogService.Application.Carts.Queries.GetCart;

public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator(ICartRepository cartRepository)
    {
        RuleFor(q => q.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Please specify a user id");
        
        RuleFor(q => q.UserId)
            .NotNull()
            .MustAsync(async (id, ct) => await cartRepository.VerifyUserAsync(id, ct))
            .When(id => id is not null)
            .WithMessage("User's cart not found");
    }
}