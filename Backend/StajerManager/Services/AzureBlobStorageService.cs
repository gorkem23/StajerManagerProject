using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StajerManager.Services
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName, string? folderPath = null);
        Task<bool> DeleteFileAsync(string blobName, string containerName);
        Task<Stream> DownloadFileAsync(string blobName, string containerName);
        Task<string> GetFileUrlAsync(string blobName, string containerName);
    }

    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly bool _isConfigured;

        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _logger = logger;
            
            try
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning("Azure Storage connection string bulunamadı.");
                    _isConfigured = false;
                    return;
                }
                
                _blobServiceClient = new BlobServiceClient(connectionString);
                _isConfigured = true;
                _logger.LogInformation("Azure Blob Storage servisi başarıyla yapılandırıldı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure Blob Storage servisi oluşturulurken hata oluştu: {Message}", ex.Message);
                _isConfigured = false;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName, string? folderPath = null)
        {
            if (!_isConfigured || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Azure Blob Storage servisi yapılandırılmamış.");
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                
                var blobName = string.IsNullOrEmpty(folderPath) 
                    ? uniqueFileName 
                    : $"{folderPath.TrimEnd('/')}/{uniqueFileName}";

                var blobClient = containerClient.GetBlobClient(blobName);

                var uploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType ?? "application/octet-stream"
                    }
                };

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, uploadOptions);
                }

                _logger.LogInformation($"Dosya başarıyla yüklendi: {blobName}");
                return blobName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Dosya yükleme hatası: {file.FileName}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string blobName, string containerName)
        {
            if (!_isConfigured || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Azure Blob Storage servisi yapılandırılmamış.");
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                
                var result = await blobClient.DeleteIfExistsAsync();
                _logger.LogInformation($"Dosya silme işlemi: {blobName}, Sonuç: {result.Value}");
                
                return result.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Dosya silme hatası: {blobName}");
                return false;
            }
        }

        public async Task<Stream> DownloadFileAsync(string blobName, string containerName)
        {
            if (!_isConfigured || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Azure Blob Storage servisi yapılandırılmamış.");
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                
                if (!await blobClient.ExistsAsync())
                {
                    throw new FileNotFoundException($"Dosya bulunamadı: {blobName}");
                }

                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Dosya indirme hatası: {blobName}");
                throw;
            }
        }

        public async Task<string> GetFileUrlAsync(string blobName, string containerName)
        {
            if (!_isConfigured || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Azure Blob Storage servisi yapılandırılmamış.");
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }
    }
}