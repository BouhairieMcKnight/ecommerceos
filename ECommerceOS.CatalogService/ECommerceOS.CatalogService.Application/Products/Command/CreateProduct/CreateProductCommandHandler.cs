namespace ECommerceOS.CatalogService.Application.Products.Command.CreateProduct;

public class CreateProductCommandHandler(
    ISkuService skuService,
    IBlobService blobService,
    ICatalogRepository catalogRepository)
    : ICommandHandler<CreateProductCommand, CreateProductCommandResponse>
{
    public async Task<Result<CreateProductCommandResponse>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = skuService.GenerateSku(request.Description)
            .Bind(s => Product.Create(sellerId: request.SellerId!, price: request.Price, sku: s,
                name: request.ProductName,
                description: request.Description,
                quantity: request.Quantity));
            
        var result = await product
            .BindAsync(async p => await GenerateSasUri(request.DirectImagesUpload, request.ProductName, blobService, cancellationToken))
            .Bind(uri => UploadImageNames(request.DirectImagesUpload, uri, request.Filenames, product.Value!))
            .TapAsync(async o => await catalogRepository.AddAsync(o.Product, cancellationToken));
        
        return result.Match(
            success => Result<CreateProductCommandResponse>
                    .Success(CreateProductCommandResponse.Create(success.Product.Id, success.Uri)), 
                Result<CreateProductCommandResponse>.Failure);
    }

    private static async Task<Result<string>> GenerateSasUri(
        bool direct, string name, IBlobService blobService, CancellationToken cancellationToken)
    {
        if (direct)
        {
            return Result<string>.Success(string.Empty);
        }
        
        var uri = await blobService.GenerateSasUri(name, cancellationToken);

        if (string.IsNullOrEmpty(uri))
        {
            return Result<string>.Failure(Error.Failure("Generating Uri", "Could not generate SasUri"));
        }
        
        return Result<string>.Success(uri);
    }

    private static Result<(Product Product, string Uri)> UploadImageNames(bool direct, string uri, List<string> filenames, Product product)
    {
        if (direct)
        {
            return Result<(Product, string)>.Success((product, uri));
        }
        
        var errors = filenames.Select(name => product.UploadImage(uri + "/" + name))
            .Where(result => !result.IsSuccess)
            .Select(result => result.Error!)
            .ToArray();

        if (errors.Any())
        {
            return Result<(Product, string)>.Failure(errors);
        }
        
        return Result<(Product, string)>.Success((product, uri));
    }
}