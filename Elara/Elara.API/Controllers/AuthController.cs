using Asp.Versioning;
using Elara.Application.Features.Auth.Commands.LoginUser;
using Elara.Application.Features.Auth.Commands.RegisterUser;
using Elara.Application.Models.Auth;
using Elara.Application.Features.Auth.Commands.RefreshToken;
using Elara.Application.Features.Auth.Commands.Logout;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Elara.Application.Features.Auth.Commands.ForgotPassword;
using Elara.API.Controllers.Requests;
using Elara.Application.Common.Interfaces;
using Elara.Application.Features.Auth.Commands.ChangePassword;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<AuthUserData>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<AuthUserData>
            {
                Message = "User registered and logged in successfully.",
                Data = result
            });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<LoginResponse>
            {
                Message = "User logged in successfully.",
                Data = result
            });
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
            return Ok(new BaseResponse<LoginResponse>
            {
                Message = "Token refreshed successfully.",
                Data = result
            });
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(BaseResponse<ForgotPasswordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<ForgotPasswordResponse>
            {
                Message = result.Message,
                Data = result
            });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok(new BaseResponse<object>
            {
                Message = "Password has been reset successfully.",
                Data = null
            });
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                return Unauthorized();

            await _mediator.Send(new ChangePasswordCommand
            {
                UserId = currentUserId.Value,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                ConfirmNewPassword = request.ConfirmNewPassword
            });

            return Ok(new BaseResponse<string>
            {
                Message = "Password changed successfully.",
                Data = null
            });
        }

        [HttpPost("logout/token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogoutByToken([FromBody] RefreshTokenRequest request)
        {
            await _mediator.Send(new LogoutCommand(request.RefreshToken));
            return Ok(new BaseResponse<string>
            {
                Message = "Logged out successfully.",
                Data = null
            });
        }
    }
}
