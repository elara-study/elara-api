using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Elara.Application.Common.Validation;

namespace Elara.API.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<bool> IsValidProfileImageAsync(this IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            var headerLength = ProfileImageValidation.MaxSignatureLength;
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

            return ProfileImageValidation.IsValidHeader(headerBytes.AsSpan(0, Math.Min(totalRead, headerLength)), file.ContentType, file.Length);
        }
    }
}
