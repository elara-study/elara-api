using Elara.Application.Models.Auth;
using Elara.Application.Models.Users;
using Elara.Application.Models.OAuth;

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

        Task<AuthUserData?> FindExistingOAuthUserAsync(OAuthUserData data);
        Task<AuthUserData> CompleteOAuthRegistrationAsync(CompleteOAuthData data);
        Task<Guid> GetUserIdByEmailAsync(string email);
        Task ResetPasswordWithOtpAsync(Guid userId, string newPassword);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task ConfirmEmailAsync(Guid userId);
        Task<string> GenerateEmailVerificationOtpAsync(Guid userId);
        Task<bool> VerifyEmailOtpAsync(Guid userId, string otp);
        Task<string> GeneratePasswordResetOtpAsync(Guid userId);
        Task<bool> VerifyPasswordResetOtpAsync(Guid userId, string otp);
        Task<string?> GetUserNameByIdAsync(Guid userId);
    }
}
