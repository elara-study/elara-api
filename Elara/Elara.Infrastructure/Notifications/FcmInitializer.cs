using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Elara.Infrastructure.Notifications
{
    public static class FcmInitializer
    {
        public static void Initialize(IConfiguration configuration, ILogger? logger = null)
        {
            if (FirebaseApp.DefaultInstance != null) return;

            var base64Credential = configuration["Firebase:ServiceAccountBase64"];

            if (string.IsNullOrWhiteSpace(base64Credential))
            {
                logger?.LogWarning("Firebase Base64 credential not found in configuration. FCM will not be available.");
                return;
            }

            try
            {
                var jsonBytes = Convert.FromBase64String(base64Credential.Trim());
                var jsonString = Encoding.UTF8.GetString(jsonBytes);

                FirebaseApp.Create(new AppOptions
                {
                    Credential = CredentialFactory.FromJson<GoogleCredential>(jsonString)
                });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to initialize Firebase from Base64 service account credential.");
            }
        }
    }
}
