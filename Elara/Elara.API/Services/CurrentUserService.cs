using System;
using System.Security.Claims;
using Elara.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Elara.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var value = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(value, out var id))
            {
                UserId = id;
            }
        }
    }
}

