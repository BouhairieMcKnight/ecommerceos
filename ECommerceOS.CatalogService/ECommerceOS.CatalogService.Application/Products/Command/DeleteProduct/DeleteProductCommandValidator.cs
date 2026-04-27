namespace ECommerceOS.CatalogService.Application.Products.Command.DeleteProduct;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator(ICatalogRepository catalogRepository)
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .WithMessage("User cannot be null");
        
        RuleFor(command => command.ProductId)
            .NotNull()
            .WithMessage("Product cannot be null");
        
        RuleFor(c => c)
            .MustAsync(async (c, ct) => 
                await catalogRepository.VerifySellerProductAsync(c.ProductId!, c.UserId!,  ct))
            .When(c => c.UserId is not null && c.ProductId is not null)
            .WithMessage("Order not found");
    }
}