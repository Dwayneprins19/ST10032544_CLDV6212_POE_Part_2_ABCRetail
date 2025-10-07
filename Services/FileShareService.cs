using Azure;
using Azure.Storage.Files.Shares;

namespace ABCRetail.Services
{
    public class FileShareService
    {
        private readonly ShareServiceClient _shareServiceClient;

        public FileShareService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        public async Task UploadFileAsync(string shareName, string filePath, Stream fileStream)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var rootDir = shareClient.GetRootDirectoryClient();
            var fileClient = rootDir.GetFileClient(filePath);

            await fileClient.CreateAsync(fileStream.Length);

            const int chunkSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[chunkSize];
            long offset = 0;
            int bytesRead;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0,buffer.Length)) > 0)
            {
                using var chunkStream = new MemoryStream(buffer, 0, bytesRead);
                await fileClient.UploadRangeAsync(
                    new HttpRange(offset, bytesRead), 
                    chunkStream
                    );
                offset += bytesRead;
            }
        }

        public async Task<Stream?> DownloadFileAsync(string shareName, string filePath)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            var rootDir = shareClient.GetRootDirectoryClient();
            var fileClient = rootDir.GetFileClient(filePath);

            if (await fileClient.ExistsAsync())
            {
                var response = await fileClient.DownloadAsync();
                var memoryStream = new MemoryStream();
                await response.Value.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            return null;
        }

        public async Task DeleteFileAsync(string shareName, string filePath)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            var rootDir = shareClient.GetRootDirectoryClient();
            var fileClient = rootDir.GetFileClient(filePath);
            await fileClient.DeleteIfExistsAsync();
        }
    }
}
