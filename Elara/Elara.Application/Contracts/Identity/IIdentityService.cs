using Elara.Application.Models.Auth;

namespace Elara.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<AuthUserData> RegisterAsync(RegisterUserData registerData);
        Task<AuthUserData?> ValidateUserCredentialsAsync(string email, string password);
    }
}
