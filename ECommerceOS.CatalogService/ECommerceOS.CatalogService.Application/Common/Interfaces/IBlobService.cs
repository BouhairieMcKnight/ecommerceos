namespace ECommerceOS.CatalogService.Application.Common.Interfaces;

public interface IBlobService
{
    Task<string> UploadImagesAsync(
        Stream stream, string containerName, string fileName, CancellationToken cancellationToken = default);
    Task<string> GenerateSasUri(string containerName, CancellationToken cancellationToken = default);
}