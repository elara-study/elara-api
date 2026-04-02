using Elara.Application.Models.Auth;
using Elara.Application.Models.Users;

namespace Elara.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<AuthUserData> RegisterAsync(RegisterUserData registerData);
        Task<AuthUserData?> ValidateUserCredentialsAsync(string email, string password);
        Task<string> GenerateRefreshTokenAsync(Guid userId, TimeSpan? expiresIn = null);
        Task<AuthUserData?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<UserImageData> GetUserImageDataAsync(Guid userId);
        Task UpdateUserImageAsync(Guid userId, string? imageUrl, string? imagePublicId);
    }
}
