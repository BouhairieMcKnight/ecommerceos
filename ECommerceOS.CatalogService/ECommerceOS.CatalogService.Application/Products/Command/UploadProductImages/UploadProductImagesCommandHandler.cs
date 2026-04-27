using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace ECommerceOS.CatalogService.Application.Products.Command.UploadProductImages;

public class UploadProductImagesCommandHandler(
    IBlobService blobService,
    ICatalogRepository catalogRepository)
    : ICommandHandler<UploadProductImagesCommand>
{
    public async Task<Result> Handle(
        UploadProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        var product = await catalogRepository.GetByIdAsync(request.ProductId, cancellationToken);
        var result = await product.BindAsync(p => BatchUploadImagesAsync(
                p,
                request.Files,
                request.ImageNames,
                cancellationToken))
            .TapAsync(async p => await catalogRepository.UpdateAsync(p, cancellationToken));
        
        return result.Match(
            success => Result.Success(),
            Result.Failure);
    }

    private async Task<Result<Product>> BatchUploadImagesAsync(
        Product product,
        IReadOnlyCollection<IFormFile> files,
        List<string> fileNames,
        CancellationToken cancellationToken)
    {
        var results = new ConcurrentBag<string>();
        await Task.WhenAll(files.Zip(fileNames).Select(async (pair) =>
        {
            var (file, fileName) = pair;
            await using (var stream = file.OpenReadStream())
            {
                var uri = await blobService.UploadImagesAsync(stream, $"{product.Name}_{product.Id}",
                    fileName, cancellationToken: cancellationToken);
                results.Add(uri);
            }
        }));
        
        var errors = results.Select(product.UploadImage)
            .Select(result => result.Error)
            .Where(error => error is not null)
            .Distinct()
            .ToArray();

        if (errors.Any())
        {
            return Result<Product>.Failure(errors!);
        }
        
        return Result<Product>.Success(product);
    }
}
