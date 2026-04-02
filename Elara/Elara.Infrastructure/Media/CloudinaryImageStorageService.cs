using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Elara.Application.Common.Interfaces;
using ElaraImageUploadResult = Elara.Application.Models.Media.ImageUploadResult;
using Microsoft.Extensions.Options;

namespace Elara.Infrastructure.Media
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(IOptions<CloudinaryOptions> options)
        {
            var settings = options.Value;

            if (string.IsNullOrWhiteSpace(settings.CloudName)
                || string.IsNullOrWhiteSpace(settings.ApiKey)
                || string.IsNullOrWhiteSpace(settings.ApiSecret)
                || settings.CloudName.StartsWith("SET_THIS", StringComparison.OrdinalIgnoreCase)
                || settings.ApiKey.StartsWith("SET_THIS", StringComparison.OrdinalIgnoreCase)
                || settings.ApiSecret.StartsWith("SET_THIS", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing. Configure Cloudinary:CloudName, Cloudinary:ApiKey, and Cloudinary:ApiSecret via environment variables or user secrets.");
            }

            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ElaraImageUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "elara/profile-images",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null)
            {
                throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
            }

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

        public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);
            if (result.Error != null)
            {
                throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");
            }
        }
    }
}
