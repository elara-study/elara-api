using Elara.Application.Contracts.Identity;
using Elara.Application.Features.Auth.Commands.OAuthCallback;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Elara.Infrastructure.Auth
{
    public class PendingTokenService : IPendingTokenService
    {
        private readonly IConfiguration _configuration;

        public PendingTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePendingToken(OAuthCallbackCommand command)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.")));

            var claims = new[]
            {
                new Claim("oauth_status", "pending_registration"),
                new Claim("provider",     command.Provider),
                new Claim("provider_uid", command.ProviderUserId),
                new Claim(JwtRegisteredClaimNames.Email, command.Email),
                new Claim(JwtRegisteredClaimNames.Name,  command.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject           = new ClaimsIdentity(claims),
                Expires           = DateTime.UtcNow.AddMinutes(15),
                Issuer            = jwtSection["Issuer"],
                Audience          = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(tokenDescriptor));
        }

        public ClaimsPrincipal? ValidatePendingToken(string token)
        {
            try
            {
                var jwtSection = _configuration.GetSection("Jwt");
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.")));

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = key,
                    ValidateIssuer           = true,
                    ValidIssuer              = jwtSection["Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = jwtSection["Audience"],
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };

                var handler = new JwtSecurityTokenHandler();
                handler.InboundClaimTypeMap.Clear();
                var principal = handler.ValidateToken(token, parameters, out _);
                return principal.FindFirstValue("oauth_status") == "pending_registration" ? principal : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
