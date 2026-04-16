using Asp.Versioning;
using Elara.API.Filters;
using Elara.Application.Features.Auth.Commands.CompleteOAuthRegistration;
using Elara.Application.Features.Auth.Commands.OAuthCallback;
using Elara.Application.Models.Auth;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("google")]
        public IActionResult LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), "OAuth", null, Request.Scheme)
            };
            return Challenge(props, "Google");
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback()
            => await HandleCallbackAsync("Google");

        [HttpGet("facebook")]
        public IActionResult LoginWithFacebook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(FacebookCallback), "OAuth", null, Request.Scheme)
            };
            return Challenge(props, "Facebook");
        }

        [HttpGet("facebook/callback")]
        public async Task<IActionResult> FacebookCallback()
            => await HandleCallbackAsync("Facebook");

        [HttpPost("complete-registration")]
        [ServiceFilter(typeof(ExtractPendingTokenClaimsFilter))]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
        {
            var claims = (ClaimsPrincipal)HttpContext.Items["PendingClaims"]!;

            var command = new CompleteOAuthRegistrationCommand
            {
                Provider       = claims.FindFirstValue("provider")     ?? string.Empty,
                ProviderUserId = claims.FindFirstValue("provider_uid") ?? string.Empty,
                Email          = claims.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty,
                Name           = claims.FindFirstValue(JwtRegisteredClaimNames.Name)  ?? string.Empty,
                Role           = request.Role,
                SubjectId      = request.SubjectId,
                DateOfBirth    = request.DateOfBirth
            };

            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<LoginResponse>
            {
                Message = "Registration completed successfully.",
                Data    = result
            });
        }

        private async Task<IActionResult> HandleCallbackAsync(string scheme)
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
                if (!result.Succeeded || result.Principal == null)
                    return Unauthorized(new BaseResponse<string>
                    {
                        Message = $"Authentication failed. Reason: {result.Failure?.Message ?? "unknown"}"
                    });

                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                var claims = result.Principal.Claims.ToList();

                var providerUserId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email          = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name           = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? email ?? "Unknown";

                if (string.IsNullOrWhiteSpace(providerUserId) || string.IsNullOrWhiteSpace(email))
                    return BadRequest(new BaseResponse<string> { Message = "Could not retrieve required profile info from provider." });

                var command = new OAuthCallbackCommand
                {
                    Provider       = scheme,
                    ProviderUserId = providerUserId,
                    Email          = email,
                    Name           = name
                };

                var response = await _mediator.Send(command);

                if (!response.IsPending)
                    return Ok(new BaseResponse<LoginResponse>
                    {
                        Message = "Logged in successfully.",
                        Data    = response.LoginResponse
                    });

                return Ok(new BaseResponse<PendingOAuthResponse>
                {
                    Message = "Please complete your registration.",
                    Data    = new PendingOAuthResponse { PendingToken = response.PendingToken! }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse<string>
                {
                    Message = $"[DEBUG] {ex.GetType().Name}: {ex.Message}"
                });
            }
        }
    }
}
