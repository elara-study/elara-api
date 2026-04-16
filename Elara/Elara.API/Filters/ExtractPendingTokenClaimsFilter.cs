using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Elara.API.Filters
{
    public class ExtractPendingTokenClaimsFilter : IAsyncActionFilter
    {
        private readonly IPendingTokenService _pendingTokenService;

        public ExtractPendingTokenClaimsFilter(IPendingTokenService pendingTokenService)
        {
            _pendingTokenService = pendingTokenService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.ActionArguments.Values
                .OfType<CompleteRegistrationRequest>()
                .FirstOrDefault();

            if (request == null)
            {
                context.Result = new BadRequestObjectResult(
                    new BaseResponse<string> { Message = "Invalid request." });
                return;
            }

            var principal = _pendingTokenService.ValidatePendingToken(request.PendingToken);

            if (principal == null)
            {
                context.Result = new UnauthorizedObjectResult(
                    new BaseResponse<string> { Message = "Invalid or expired pending token." });
                return;
            }

            context.HttpContext.Items["PendingClaims"] = principal;

            await next();
        }
    }
}
