using Elara.Application.Contracts.persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.UpdatePreferences
{
    public class UpdatePreferencesCommandHandler : IRequestHandler<UpdatePreferencesCommand>
    {
        private readonly INotificationPreferenceRepository _preferenceRepository;

        public UpdatePreferencesCommandHandler(INotificationPreferenceRepository preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        public async Task Handle(UpdatePreferencesCommand request, CancellationToken cancellationToken)
        {
            var existing = await _preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (existing == null)
            {
                var pref = new NotificationPreference
                {
                    UserId = request.UserId,
                    GroupUpdates = request.GroupUpdates,
                    StreakReminders = request.StreakReminders,
                    HomeworkReminders = request.HomeworkReminders,
                    NewLessons = request.NewLessons,
                    AiProgressReports = request.AiProgressReports
                };
                await _preferenceRepository.AddAsync(pref, cancellationToken);
            }
            else
            {
                existing.GroupUpdates = request.GroupUpdates;
                existing.StreakReminders = request.StreakReminders;
                existing.HomeworkReminders = request.HomeworkReminders;
                existing.NewLessons = request.NewLessons;
                existing.AiProgressReports = request.AiProgressReports;
                await _preferenceRepository.UpdateAsync(existing, cancellationToken);
            }
        }
    }
}
