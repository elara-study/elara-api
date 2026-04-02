using Elara.Application.Models.Media;

namespace Elara.Application.Common.Interfaces
{
    public interface IImageStorageService
    {
        Task<ImageUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
    }
}
