using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.CompleteOAuthRegistration
{
    public class CompleteOAuthRegistrationCommand : IRequest<LoginResponse>
    {
        // Populated by the controller from the validated pending token claims
        public string Provider       { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
        public string Name           { get; set; } = string.Empty;

        // Provided by the client in the request body
        public string Role      { get; set; } = string.Empty;
        public int?   SubjectId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
