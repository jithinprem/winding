using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using windingApi.Services.Interfaces;

namespace windingApi.Services;

// blob blog: https://www.zuar.com/blog/azure-blob-storage-cheat-sheet/#:~:text=One%20way%20to%20find%20the,URL%20directly%20to%20the%20clipboard.
public class AzureBlobService
{

    private readonly BlobContainerClient _containerClient;
    
    public AzureBlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }
    
    public async Task<string> UploadBlobAsync(string blobName, Stream content)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);
        return blobClient.Uri.ToString(); // return the blob URL
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteAsync();
    }

    public async Task<string> GetBlobContentAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var downloadInfo = await blobClient.DownloadAsync();
        if (downloadInfo == null) return "";
        using var reader = new StreamReader(downloadInfo.Value.Content);
        return await reader.ReadToEndAsync();
    }
}