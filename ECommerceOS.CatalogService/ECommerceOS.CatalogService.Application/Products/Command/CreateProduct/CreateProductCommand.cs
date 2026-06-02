namespace ECommerceOS.CatalogService.Application.Products.Command.CreateProduct;

public record CreateProductCommand(
    string ProductName,
    string Description,
    Money Price,
    bool DirectImagesUpload,
    UserId? SellerId,
    int Quantity,
    List<string> Filenames
) : ICommand<CreateProductCommandResponse>;

public record CreateProductCommandResponse(ProductId ProductId, string UploadUri)
{
    public static CreateProductCommandResponse Create(ProductId productId, string directImagesUpload)
    {
        return new(productId, directImagesUpload);
    }
};
