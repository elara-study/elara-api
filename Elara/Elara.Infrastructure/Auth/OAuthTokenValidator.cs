using Elara.Application.Contracts.Identity;
using Elara.Application.Models.OAuth;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Elara.Infrastructure.Auth
{
    public class OAuthTokenValidator : IOAuthTokenValidator
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OAuthTokenValidator(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient    = httpClient;
        }

        public async Task<OAuthUserInfo> ValidateGoogleTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["OAuth:Google:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new OAuthUserInfo
            {
                ProviderUserId = payload.Subject,
                Email          = payload.Email,
                Name           = payload.Name ?? payload.Email
            };
        }

        public async Task<OAuthUserInfo> ValidateFacebookTokenAsync(string accessToken)
        {
            var appId     = _configuration["OAuth:Facebook:AppId"]!;
            var appSecret = _configuration["OAuth:Facebook:AppSecret"]!;

            var appAccessToken = $"{appId}|{appSecret}";
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken}";
            
            var debugResponse = await _httpClient.GetAsync(debugUrl);
            if (!debugResponse.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to validate Facebook token.");

            var debugResult = await debugResponse.Content.ReadFromJsonAsync<FacebookDebugTokenResponse>();
            if (debugResult?.Data == null || !debugResult.Data.IsValid || debugResult.Data.AppId != appId)
                throw new InvalidOperationException("Invalid Facebook token or token not issued for this app.");

            var url = $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to fetch Facebook user data.");

            var json = await response.Content.ReadFromJsonAsync<FacebookUserResponse>();
            if (json == null || string.IsNullOrWhiteSpace(json.Email))
                throw new InvalidOperationException("Facebook did not return a valid email.");

            return new OAuthUserInfo
            {
                ProviderUserId = json.Id,
                Email          = json.Email,
                Name           = json.Name ?? json.Email
            };
        }
    }
}
