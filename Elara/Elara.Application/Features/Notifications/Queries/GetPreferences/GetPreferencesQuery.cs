using MediatR;

namespace Elara.Application.Features.Notifications.Queries.GetPreferences
{
    public class GetPreferencesQuery : IRequest<PreferencesDto>
    {
        public Guid UserId { get; }

        public GetPreferencesQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
