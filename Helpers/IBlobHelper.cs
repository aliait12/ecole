using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Helpers
{
    public interface IBlobHelper
    {
        // Upload file via form
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        // File upload via byte array
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);

        // Upload file via URL
        Task<Guid> UploadBlobAsync(string image, string containerName);
    }
}