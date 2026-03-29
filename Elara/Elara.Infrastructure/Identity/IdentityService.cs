using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Domain.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Elara.Infrastructure.Data;
using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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

            var localTracked = _context.Set<IdentityUserToken<Guid>>().Local
                .FirstOrDefault(t => t.UserId == user.Id && t.LoginProvider == RefreshTokenProvider && t.Name == tokenName);
            if (localTracked != null)
            {
                _context.Entry(localTracked).State = EntityState.Detached;
            }

            var existing = await _context.Set<IdentityUserToken<Guid>>()
                .FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == RefreshTokenProvider && t.Name == tokenName);

            if (existing == null)
            {
                _context.Set<IdentityUserToken<Guid>>().Add(new IdentityUserToken<Guid>
                {
                    UserId = user.Id,
                    LoginProvider = RefreshTokenProvider,
                    Name = tokenName,
                    Value = value
                });
            }
            else
            {
                existing.Value = value;
            }

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
    }
}
