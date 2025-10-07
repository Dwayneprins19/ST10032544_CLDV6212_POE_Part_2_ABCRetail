using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task UploadBlobAsync(string containerName, Stream fileStream, string fileName)
        { 
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream, overwrite: true);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Blob uplaod failed: {ex.Message}");
                throw;
            }
            
        }

        public string GetBlobUrl(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Blob delete failed: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> BlobExistsAsync(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);
            return await blobClient.ExistsAsync(); 
        }

        public async Task<List<string>> ListBlobAsync(string containerName)
        {
            var result = new List<string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
                result.Add(blob.Name);
            }
            return result;
        }
        
        public async Task<Stream?> DownloadBlobAsync(string containerName, string fileName) 
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToLower());
                var blobClient = containerClient.GetBlobClient(fileName);

                if (await blobClient.ExistsAsync())
                {
                    var response = await blobClient.DownloadAsync();
                    return response.Value.Content;
                }
                return null;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Blob download failed: {ex.Message}");
                throw;
            }
        }
    }
}
