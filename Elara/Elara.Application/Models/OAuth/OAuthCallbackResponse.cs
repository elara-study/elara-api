using Elara.Application.Models.Auth;

namespace Elara.Application.Models.OAuth
{
    public class OAuthCallbackResponse
    {
        public bool IsPending { get; set; }
        public LoginResponse? LoginResponse { get; set; }
        public string? PendingToken { get; set; }
    }
}
