using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Domain.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AuthUserData> RegisterAsync(RegisterUserData registerData)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerData.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = registerData.Email,
                Email = registerData.Email,
                Name = registerData.Name,
                DateOfBirth = registerData.DateOfBirth,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerData.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", registerData.Email, errors);
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Teacher);
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to assign default role for {Email}. Errors: {Errors}", registerData.Email, roleErrors);
                throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
            }

            _logger.LogInformation("User {Email} registered successfully with ID {UserId}", registerData.Email, user.Id);

            var authUser = _mapper.Map<AuthUserData>(user);
            authUser.Role = Roles.Teacher;
            return authUser;
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
    }
}
