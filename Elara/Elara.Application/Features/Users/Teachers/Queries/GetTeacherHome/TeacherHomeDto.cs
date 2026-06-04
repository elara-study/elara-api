namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherHome
{
    public class TeacherHomeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public List<TeacherGroupDto> Groups { get; set; } = new();
        public List<TeacherRoadmapDto> Roadmaps { get; set; } = new();
        public TeacherStatsDto Stats { get; set; } = new();
        public List<TeacherPendingTaskDto> PendingTasks { get; set; } = new();
        public List<TeacherRecentActivityDto> RecentActivity { get; set; } = new();
    }

    public class TeacherGroupDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
    }

    public class TeacherRoadmapDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int LessonsCount { get; set; }
        public int Grade { get; set; }
    }

    public class TeacherStatsDto
    {
        public ActiveStudentsStatDto ActiveStudents { get; set; } = new();
        public AvgCompletionStatDto AvgCompletion { get; set; } = new();
    }

    public class ActiveStudentsStatDto
    {
        public int Count { get; set; }
        public int Delta { get; set; }
    }

    public class AvgCompletionStatDto
    {
        public int Percentage { get; set; }
        public int Delta { get; set; }
    }

    public class TeacherPendingTaskDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class TeacherRecentActivityDto
    {
        public ActivityStudentDto Student { get; set; } = new();
        public string Type { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class ActivityStudentDto
    {
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
