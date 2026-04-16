using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;

namespace Elara.Application.Contracts.Identity
{
    public interface IOAuthTokenValidator
    {
        Task<OAuthUserInfo> ValidateGoogleTokenAsync(string idToken);
        Task<OAuthUserInfo> ValidateFacebookTokenAsync(string accessToken);
    }
}
