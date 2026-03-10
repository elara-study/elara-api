using Elara.Application.Models.Auth;

namespace Elara.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<AuthUserData> RegisterAsync(string email, string name, DateTime? dateOfBirth, string password);
    }
}
