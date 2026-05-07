using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Notifications
{
    public static class FcmInitializer
    {
        public static void Initialize(IConfiguration configuration, ILogger? logger = null)
        {
            if (FirebaseApp.DefaultInstance != null) return;

            var credentialPath = configuration["Firebase:CredentialPath"];

            if (string.IsNullOrWhiteSpace(credentialPath) || !File.Exists(credentialPath))
            {
                logger?.LogWarning("Firebase credential file not found at '{Path}'. FCM will not be available.", credentialPath);
                return;
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
        }
    }
}
