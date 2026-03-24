using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Elara.Infrastructure.Auth
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(AuthUserData userData)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(
                jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured."));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userData.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userData.Email),
                new Claim(JwtRegisteredClaimNames.Name, userData.Name),
                new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
                new Claim(ClaimTypes.Role, userData.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    double.TryParse(jwtSection["ExpireMinutes"], out var minutes) ? minutes : 60),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
