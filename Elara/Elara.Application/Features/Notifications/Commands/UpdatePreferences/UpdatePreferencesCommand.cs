using MediatR;

namespace Elara.Application.Features.Notifications.Commands.UpdatePreferences
{
    public class UpdatePreferencesCommand : IRequest
    {
        public Guid UserId { get; }
        public bool GroupUpdates { get; }
        public bool StreakReminders { get; }
        public bool HomeworkReminders { get; }
        public bool NewLessons { get; }
        public bool AiProgressReports { get; }

        public UpdatePreferencesCommand(Guid userId, bool groupUpdates, bool streakReminders,
            bool homeworkReminders, bool newLessons, bool aiProgressReports)
        {
            UserId = userId;
            GroupUpdates = groupUpdates;
            StreakReminders = streakReminders;
            HomeworkReminders = homeworkReminders;
            NewLessons = newLessons;
            AiProgressReports = aiProgressReports;
        }
    }
}
