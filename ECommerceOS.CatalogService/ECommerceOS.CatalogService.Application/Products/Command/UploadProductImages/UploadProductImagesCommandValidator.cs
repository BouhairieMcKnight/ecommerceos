namespace ECommerceOS.CatalogService.Application.Products.Command.UploadProductImages;

public class UploadProductImagesCommandValidator : AbstractValidator<UploadProductImagesCommand>
{
    public UploadProductImagesCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty()
            .WithMessage("Product Id is required.");
        
        RuleFor(command => command)
            .Must(command => command.Files.Count == command.ImageNames.Count)
            .WithMessage("Files count must match image names count.");
            
    }
}
