namespace Elara.Application.Models.Auth
{
    public class OAuthCallbackResponse
    {
        public bool IsPending { get; set; }
        public LoginResponse? LoginResponse { get; set; }
        public string? PendingToken { get; set; }
    }
}
