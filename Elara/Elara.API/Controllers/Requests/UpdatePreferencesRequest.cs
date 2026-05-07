namespace Elara.API.Controllers.Requests
{
    public class UpdatePreferencesRequest
    {
        public bool GroupUpdates { get; set; }
        public bool StreakReminders { get; set; }
        public bool HomeworkReminders { get; set; }
        public bool NewLessons { get; set; }
        public bool AiProgressReports { get; set; }
    }
}
