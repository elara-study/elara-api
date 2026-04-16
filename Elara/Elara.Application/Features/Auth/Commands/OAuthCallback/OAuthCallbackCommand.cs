using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.OAuthCallback
{
    public class OAuthCallbackCommand : IRequest<OAuthCallbackResponse>
    {
        public string Provider       { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
        public string Name           { get; set; } = string.Empty;
    }
}
