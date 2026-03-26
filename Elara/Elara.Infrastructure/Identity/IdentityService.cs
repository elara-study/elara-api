using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Domain.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Elara.Infrastructure.Data;
using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
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
    }
}
