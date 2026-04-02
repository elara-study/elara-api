using System;
using System.Linq;

namespace Elara.Application.Common.Validation
{
    public static class ProfileImageValidation
    {
        public const long MaxImageSizeBytes = 5 * 1024 * 1024;
        public static readonly string[] AllowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

        public static bool IsAllowedContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        }

        public static bool HasValidSignature(ReadOnlySpan<byte> fileBytes, string contentType)
        {
            if (fileBytes.Length < 12)
            {
                return false;
            }

            var jpeg = contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
                && fileBytes[0] == 0xFF
                && fileBytes[1] == 0xD8
                && fileBytes[2] == 0xFF;

            if (jpeg)
            {
                return true;
            }

            var png = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
                && fileBytes[0] == 0x89
                && fileBytes[1] == 0x50
                && fileBytes[2] == 0x4E
                && fileBytes[3] == 0x47
                && fileBytes[4] == 0x0D
                && fileBytes[5] == 0x0A
                && fileBytes[6] == 0x1A
                && fileBytes[7] == 0x0A;

            if (png)
            {
                return true;
            }

            var webp = contentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase)
                && fileBytes[0] == 0x52
                && fileBytes[1] == 0x49
                && fileBytes[2] == 0x46
                && fileBytes[3] == 0x46
                && fileBytes[8] == 0x57
                && fileBytes[9] == 0x45
                && fileBytes[10] == 0x42
                && fileBytes[11] == 0x50;

            return webp;
        }
    }
}