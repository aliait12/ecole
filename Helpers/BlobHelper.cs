using Microsoft.AspNetCore.Http;

namespace SchoolManagementSystem.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        public BlobHelper(IConfiguration configuration)
        {
            // Azure Blob volontairement désactivé
        }

        public Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            return Task.FromResult(Guid.Empty);
        }

        public Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            return Task.FromResult(Guid.Empty);
        }

        public Task<Guid> UploadBlobAsync(string base64, string containerName)
        {
            return Task.FromResult(Guid.Empty);
        }

        public Task DeleteBlobAsync(Guid blobId, string containerName)
        {
            return Task.CompletedTask;
        }
    }
}
