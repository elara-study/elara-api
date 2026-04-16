using Asp.Versioning;
using Elara.Application.Contracts.Identity;
using Elara.Application.Features.Auth.Commands.CompleteOAuthRegistration;
using Elara.Application.Features.Auth.Commands.OAuthCallback;
using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;
using Elara.Application.Responses;
using MediatR;
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
        private readonly IMediator            _mediator;
        private readonly IOAuthTokenValidator  _tokenValidator;
        private readonly IPendingTokenService  _pendingTokenService;

        public OAuthController(IMediator mediator,IOAuthTokenValidator tokenValidator,IPendingTokenService pendingTokenService)
        {
            _mediator            = mediator;
            _tokenValidator      = tokenValidator;
            _pendingTokenService = pendingTokenService;
        }

        [HttpPost("google")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalTokenRequest request)
        {
            var userInfo = await _tokenValidator.ValidateGoogleTokenAsync(request.Token);
            return await HandleOAuthAsync("Google", userInfo);
        }

        [HttpPost("facebook")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FacebookLogin([FromBody] ExternalTokenRequest request)
        {
            var userInfo = await _tokenValidator.ValidateFacebookTokenAsync(request.Token);
            return await HandleOAuthAsync("Facebook", userInfo);
        }

        [HttpPost("complete-registration")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
        {
            var claims = _pendingTokenService.ValidatePendingToken(request.PendingToken);
            if (claims == null)
                return Unauthorized(new BaseResponse<string> { Message = "Invalid or expired pending token." });

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

        private async Task<IActionResult> HandleOAuthAsync(string provider, OAuthUserInfo userInfo)
        {
            var command = new OAuthCallbackCommand
            {
                Provider       = provider,
                ProviderUserId = userInfo.ProviderUserId,
                Email          = userInfo.Email,
                Name           = userInfo.Name
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
    }
}
