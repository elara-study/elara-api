using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AuthUserData> RegisterAsync(string email, string name, DateTime? dateOfBirth, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                DateOfBirth = dateOfBirth,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", email, errors);
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            _logger.LogInformation("User {Email} registered successfully with ID {UserId}", email, user.Id);

            return new AuthUserData
            {
                UserId = user.Id,
                Email = user.Email!,
                Name = user.Name
            };
        }
    }
}
