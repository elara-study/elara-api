using Asp.Versioning;
using Elara.Application.Features.Auth.Commands.LoginUser;
using Elara.Application.Features.Auth.Commands.RegisterUser;
using Elara.Application.Models.Auth;
using Elara.Application.Features.Auth.Commands.RefreshToken;
using Elara.Application.Features.Auth.Commands.Logout;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<AuthUserData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<AuthUserData>
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
