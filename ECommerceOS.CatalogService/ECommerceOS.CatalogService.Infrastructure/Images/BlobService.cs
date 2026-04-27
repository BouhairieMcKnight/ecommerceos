using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;

namespace ECommerceOS.CatalogService.Infrastructure.Images;

public class BlobService(BlobServiceClient blobServiceClient) : IBlobService
{
    public async Task<string> UploadImagesAsync(Stream stream, string containerName, string fileName,
        CancellationToken cancellationToken = default)
    {
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName);
        await blobClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        try
        {
            await blobClient.UploadBlobAsync(fileName, stream, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(fileName).Uri.AbsoluteUri;
    }

    public async Task<string> GenerateSasUri(string containerName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient? containerClient = null;
        try
        {
            containerClient = await blobServiceClient
                .CreateBlobContainerAsync(
                    containerName, PublicAccessType.Blob, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            return string.Empty;
        } 
        
        var sasBuilder = new BlobSasBuilder
        {
            Resource = "c", 
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10),
        };
    
        sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.List);
        
        
        Uri? sasUri;
        try
        {
            sasUri = containerClient.GenerateSasUri(sasBuilder);
        }
        catch(Exception ex)
        {
            return string.Empty;
        }

        return sasUri.AbsoluteUri;
    }
}
