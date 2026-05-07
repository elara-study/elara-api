using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Queries.GetPreferences
{
    public class GetPreferencesQueryHandler : IRequestHandler<GetPreferencesQuery, PreferencesDto>
    {
        private readonly INotificationPreferenceRepository _preferenceRepository;

        public GetPreferencesQueryHandler(INotificationPreferenceRepository preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        public async Task<PreferencesDto> Handle(GetPreferencesQuery request, CancellationToken cancellationToken)
        {
            var pref = await _preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (pref == null)
            {
                return new PreferencesDto
                {
                    GroupUpdates = true,
                    StreakReminders = true,
                    HomeworkReminders = true,
                    NewLessons = true,
                    AiProgressReports = true
                };
            }

            return new PreferencesDto
            {
                GroupUpdates = pref.GroupUpdates,
                StreakReminders = pref.StreakReminders,
                HomeworkReminders = pref.HomeworkReminders,
                NewLessons = pref.NewLessons,
                AiProgressReports = pref.AiProgressReports
            };
        }
    }
}
