using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Elara.Application.Common.Validation;

namespace Elara.API.Extensions
{
    public static class FormFileExtensions
    {
        /// <summary>
        /// Lightweight header-only validation for profile images.
        /// Returns false for null/empty files, disallowed types, oversized files,
        /// or when the file's magic bytes do not match the declared content type.
        /// </summary>
        public static async Task<bool> IsValidProfileImageAsync(this IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (file.Length > ProfileImageValidation.MaxImageSizeBytes)
            {
                return false;
            }

            if (!ProfileImageValidation.IsAllowedContentType(file.ContentType))
            {
                return false;
            }

            const int headerLength = 12;
            var headerBytes = new byte[headerLength];
            var totalRead = 0;

            try
            {
                using var stream = file.OpenReadStream();
                while (totalRead < headerLength)
                {
                    var bytesRead = await stream.ReadAsync(headerBytes.AsMemory(totalRead, headerLength - totalRead));
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    totalRead += bytesRead;
                }
            }
            catch
            {
                // If we can't read the stream, consider it invalid
                return false;
            }

            if (totalRead == 0)
            {
                return false;
            }

            return ProfileImageValidation.HasValidSignature(headerBytes.AsSpan(0, Math.Min(totalRead, headerLength)), file.ContentType ?? string.Empty);
        }
    }
}
