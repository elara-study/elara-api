using Elara.Application.Models.Auth;

namespace Elara.Application.Contracts.Identity
{
    public interface ITokenService
    {
        string CreateToken(AuthUserData userData);
    }
}
