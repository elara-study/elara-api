using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;
using Elara.Domain.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Elara.Infrastructure.Data;
using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Elara.Application.Models.Users;

namespace Elara.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private const string RefreshTokenProvider = "Elara";
        private const string RefreshTokenNamePrefix = "RefreshToken:";

        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly AppDbContext _context;

        public IdentityService(IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger, AppDbContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<AuthUserData> RegisterAsync(RegisterUserData registerData)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerData.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new ApplicationUser
                {
                    UserName = registerData.Email,
                    Email = registerData.Email,
                    Name = registerData.Name,
                    DateOfBirth = DateTime.SpecifyKind(registerData.DateOfBirth, DateTimeKind.Utc),
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, registerData.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", registerData.Email, errors);
                    throw new InvalidOperationException($"Registration failed: {errors}");
                }

                var roleInput = registerData.Role?.Trim().ToLower();

                string requestedRole = roleInput switch
                {
                    "teacher" => Roles.Teacher,
                    "parent" => Roles.Parent,
                    "student" => Roles.Student,
                    "admin" => throw new UnauthorizedAccessException("Cannot register as Admin through public registration."),
                    _ => string.IsNullOrWhiteSpace(roleInput) ? Roles.Student : throw new InvalidOperationException($"Role {roleInput} does not exist.")
                };
                var roleResult = await _userManager.AddToRoleAsync(user, requestedRole);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to assign role for {Email}. Errors: {Errors}", registerData.Email, roleErrors);
                    throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
                }

                if (requestedRole == Roles.Teacher)
                {
                    if (!registerData.SubjectId.HasValue || registerData.SubjectId.Value <= 0)
                        throw new InvalidOperationException("SubjectId is required for teachers.");

                    var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == registerData.SubjectId.Value);
                    if (!subjectExists)
                        throw new InvalidOperationException($"Subject with id {registerData.SubjectId.Value} does not exist.");

                    var newTeacher = new Teacher
                    {
                        Id = user.Id,
                        SubjectId = registerData.SubjectId.Value,
                        IsDeleted = false
                    };
                    await _context.Teachers.AddAsync(newTeacher);
                }
                else if (requestedRole == Roles.Student)
                {
                    var newStudent = new Student
                    {
                        Id = user.Id,
                        IsDeleted = false
                    };
                    await _context.Students.AddAsync(newStudent);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {Email} registered successfully with ID {UserId}", registerData.Email, user.Id);

                var authUser = _mapper.Map<AuthUserData>(user);
                authUser.Role = requestedRole;
                return authUser;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AuthUserData?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedAccessException("Please verify your email address to log in.");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(role))
            {
                return null;
            }

            var authUser = _mapper.Map<AuthUserData>(user);
            authUser.Role = role;
            return authUser;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId, TimeSpan? expiresIn = null)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");

            await CleanupExpiredRefreshTokensAsync(user.Id);

            var tokenBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);

            var expiry = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromDays(7));
            var tokenName = BuildRefreshTokenName(token);
            var value = expiry.Ticks.ToString();

            var activeTokens = await _context.Set<IdentityUserToken<Guid>>()
                .Where(t => t.UserId == user.Id && t.LoginProvider == RefreshTokenProvider && t.Name.StartsWith(RefreshTokenNamePrefix))
                .ToListAsync();

            if (activeTokens.Count >= 5)
            {
                var oldestTokens = activeTokens.OrderBy(t => t.Value).Take(activeTokens.Count - 4);
                _context.Set<IdentityUserToken<Guid>>().RemoveRange(oldestTokens);
            }

            _context.Set<IdentityUserToken<Guid>>().Add(new IdentityUserToken<Guid>
            {
                UserId = user.Id,
                LoginProvider = RefreshTokenProvider,
                Name = tokenName,
                Value = value
            });

            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<AuthUserData?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var tokenName = BuildRefreshTokenName(refreshToken);
            var tokenEntity = await _context.Set<IdentityUserToken<Guid>>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.LoginProvider == RefreshTokenProvider && t.Name == tokenName);

            if (tokenEntity == null)
                return null;

            if (!long.TryParse(tokenEntity.Value, out var expiryTicks))
            {
                await _context.Set<IdentityUserToken<Guid>>()
                    .Where(t => t.UserId == tokenEntity.UserId && t.LoginProvider == tokenEntity.LoginProvider && t.Name == tokenEntity.Name)
                    .ExecuteDeleteAsync();
                return null;
            }

            var expiry = new DateTime(expiryTicks, DateTimeKind.Utc);
            if (expiry <= DateTime.UtcNow)
            {
                await _context.Set<IdentityUserToken<Guid>>()
                    .Where(t => t.UserId == tokenEntity.UserId && t.LoginProvider == tokenEntity.LoginProvider && t.Name == tokenEntity.Name)
                    .ExecuteDeleteAsync();
                return null;
            }

            var user = await _userManager.FindByIdAsync(tokenEntity.UserId.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var authUser = _mapper.Map<AuthUserData>(user);
            authUser.Role = role ?? string.Empty;
            return authUser;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var tokenName = BuildRefreshTokenName(refreshToken);
            var affectedRows = await _context.Set<IdentityUserToken<Guid>>()
                .Where(t => t.LoginProvider == RefreshTokenProvider && t.Name == tokenName)
                .ExecuteDeleteAsync();

            return affectedRows > 0;
        }

        public async Task<UserImageData> GetUserImageDataAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            }

            return new UserImageData
            {
                UserId = user.Id,
                ImageUrl = user.ImageUrl,
                ImagePublicId = user.ImagePublicId
            };
        }

        public async Task UpdateUserImageAsync(Guid userId, string? imageUrl, string? imagePublicId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            }

            user.ImageUrl = imageUrl;
            user.ImagePublicId = imagePublicId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user image: {errors}");
            }
        }

        private static string BuildRefreshTokenName(string refreshToken)
        {
            var hashed = ComputeSha256(refreshToken);
            return $"{RefreshTokenNamePrefix}{hashed}";
        }

        private static string ComputeSha256(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private async Task CleanupExpiredRefreshTokensAsync(Guid? userId = null)
        {
            var query = _context.Set<IdentityUserToken<Guid>>()
                .AsNoTracking()
                .Where(t => t.LoginProvider == RefreshTokenProvider && t.Name.StartsWith(RefreshTokenNamePrefix));

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            var tokens = await query
                .Select(t => new { t.UserId, t.LoginProvider, t.Name, t.Value })
                .ToListAsync();

            var expired = tokens
                .Where(t => !long.TryParse(t.Value, out var ticks) || new DateTime(ticks, DateTimeKind.Utc) <= DateTime.UtcNow)
                .ToList();

            foreach (var token in expired)
            {
                await _context.Set<IdentityUserToken<Guid>>()
                    .Where(t => t.UserId == token.UserId && t.LoginProvider == token.LoginProvider && t.Name == token.Name)
                    .ExecuteDeleteAsync();
            }
        }

        public async Task<AuthUserData?> FindExistingOAuthUserAsync(OAuthUserData data)
        {
            var user = await _userManager.FindByLoginAsync(data.Provider, data.ProviderUserId);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(data.Email);
                if (user == null)
                    return null;

                var loginInfo = new UserLoginInfo(data.Provider, data.ProviderUserId, data.Provider);
                await _userManager.AddLoginAsync(user, loginInfo);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var authUser = _mapper.Map<AuthUserData>(user);
            authUser.Role = roles.FirstOrDefault() ?? string.Empty;
            return authUser;
        }

        public async Task<AuthUserData> CompleteOAuthRegistrationAsync(CompleteOAuthData data)
        {
            var roleInput = data.Role.Trim().ToLower();
            string requestedRole = roleInput switch
            {
                "teacher" => Roles.Teacher,
                "student" => Roles.Student,
                _ => throw new InvalidOperationException($"Role '{roleInput}' is not valid for OAuth registration.")
            };

            if (requestedRole == Roles.Teacher)
            {
                if (!data.SubjectId.HasValue || data.SubjectId.Value <= 0)
                    throw new InvalidOperationException("SubjectId is required for teachers.");

                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == data.SubjectId.Value);
                if (!subjectExists)
                    throw new InvalidOperationException($"Subject with id {data.SubjectId.Value} does not exist.");
            }

            var user = new ApplicationUser
            {
                UserName       = data.Email,
                Email          = data.Email,
                Name           = data.Name,
                DateOfBirth    = DateTime.SpecifyKind(data.DateOfBirth, DateTimeKind.Utc),
                EmailConfirmed = true
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"OAuth user creation failed: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, requestedRole);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Role assignment failed: {errors}");
                }

                if (requestedRole == Roles.Teacher)
                    _context.Teachers.Add(new Teacher { Id = user.Id, SubjectId = data.SubjectId!.Value, IsDeleted = false });
                else
                    _context.Students.Add(new Student { Id = user.Id, IsDeleted = false });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var loginInfo = new UserLoginInfo(data.Provider, data.ProviderUserId, data.Provider);
            await _userManager.AddLoginAsync(user, loginInfo);

            _logger.LogInformation("OAuth user {Email} registered via {Provider} with role {Role}", data.Email, data.Provider, requestedRole);

            var authUser = _mapper.Map<AuthUserData>(user);
            authUser.Role = requestedRole;
            return authUser;
        }

        public async Task<Guid> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("No account found with this email.");
            return user.Id;
        }

        public async Task ResetPasswordWithOtpAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }
        }
        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} was not found.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password change failed: {errors}");
            }
        }

        public async Task ConfirmEmailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} was not found.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Email confirmation failed: {errors}");
            }
        }
    }
}
