namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupOverview
{
    public sealed class GetStudentGroupOverviewResponse
    {
        public StudentGroupOverviewGroup Group { get; init; } = new();
        public StudentGroupOverviewProgress Progress { get; init; } = new();
    }

    public sealed class StudentGroupOverviewGroup
    {
        public string Name { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public int Grade { get; init; }
    }

    public sealed class StudentGroupOverviewProgress
    {
        public int CurrentLesson { get; init; }
        public int TotalLessons { get; init; }
    }
}
