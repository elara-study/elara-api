using Elara.Application.Features.Auth.Commands.OAuthCallback;
using System.Security.Claims;

namespace Elara.Application.Contracts.Identity
{
    public interface IPendingTokenService
    {
        string CreatePendingToken(OAuthCallbackCommand command);
        ClaimsPrincipal? ValidatePendingToken(string token);
    }
}
