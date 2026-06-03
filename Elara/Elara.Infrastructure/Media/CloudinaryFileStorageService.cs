using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Elara.Application.Common.Interfaces;
using ElaraImageUploadResult = Elara.Application.Models.Media.ImageUploadResult;
using Microsoft.Extensions.Options;

namespace Elara.Infrastructure.Media
{
    public class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorageService(IOptions<CloudinaryOptions> options)
        {
            var settings = options.Value;
            if (string.IsNullOrWhiteSpace(settings.CloudName)
                 || string.IsNullOrWhiteSpace(settings.ApiKey)
                 || string.IsNullOrWhiteSpace(settings.ApiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing. Configure Cloudinary:CloudName, Cloudinary:ApiKey, and Cloudinary:ApiSecret via environment variables or user secrets.");
            }
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ElaraImageUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var resourceType = GetResourceType(contentType);
            
            RawUploadParams uploadParams;
            if (resourceType == ResourceType.Video)
            {
                uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "elara/videos",
                    UseFilename = false,
                    UniqueFilename = true
                };
            }
            else if (resourceType == ResourceType.Image)
            {
                uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "elara/images",
                    UseFilename = false,
                    UniqueFilename = true
                };
            }
            else 
            {
                uploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "elara/documents",
                    UseFilename = false,
                    UniqueFilename = true
                };
            }

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null) throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

            if (string.IsNullOrWhiteSpace(result.SecureUrl?.ToString()) || string.IsNullOrWhiteSpace(result.PublicId))
            {
                throw new InvalidOperationException("Cloudinary upload failed to return URL or public ID.");
            }

            return new ElaraImageUploadResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task DeleteAsync(string publicId, string resourceType = "image", CancellationToken cancellationToken = default)
        {
            var type = resourceType.ToLower() switch
            {
                "video" => ResourceType.Video,
                "raw" => ResourceType.Raw,
                "pdf" => ResourceType.Raw,
                _ => ResourceType.Image
            };

            var deletionParams = new DeletionParams(publicId) { ResourceType = type };
            var result = await _cloudinary.DestroyAsync(deletionParams);
            if (result.Error != null)
            {
                throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");
            }
        }

        private ResourceType GetResourceType(string contentType)
        {
            if (contentType.StartsWith("video/")) return ResourceType.Video;
            if (contentType.StartsWith("image/")) return ResourceType.Image;
            return ResourceType.Raw;
        }
    }
}
