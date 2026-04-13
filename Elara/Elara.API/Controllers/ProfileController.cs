using Asp.Versioning;
using Elara.API.Controllers.Requests;
using Elara.Application.Common.Interfaces;
using Elara.Application.Features.Users.Profile.Commands.DeleteProfileImage;
using Elara.Application.Features.Users.Profile.Commands.UpdateProfileImage;
using Elara.Application.Responses;
using Elara.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPut("image")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<UpdateProfileImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfileImage([FromForm] UpdateProfileImageRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            if (request.File == null)
            {
                return BadRequest(new BaseResponse<UpdateProfileImageResponse>
                {
                    Message = "A valid image file is required."
                });
            }

            if (!await request.File.IsValidProfileImageAsync())
            {
                return BadRequest(new BaseResponse<UpdateProfileImageResponse>
                {
                    Message = "Invalid image file or signature."
                });
            }

            byte[] bytes;
            using (var fullStream = request.File.OpenReadStream())
            using (var memoryStream = new MemoryStream())
            {
                await fullStream.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            var command = new UpdateProfileImageCommand(currentUserId.Value, bytes, request.File.FileName, request.File.ContentType);
            var result = await _mediator.Send(command);

            return Ok(new BaseResponse<UpdateProfileImageResponse>
            {
                Message = "Profile image updated successfully.",
                Data = result
            });
        }

        [HttpDelete("image")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            await _mediator.Send(new DeleteProfileImageCommand(currentUserId.Value));

            return NoContent();
        }
    }
}
