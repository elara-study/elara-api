namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleHomeworks
{
    public class ModuleHomeworkDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
