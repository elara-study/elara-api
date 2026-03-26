namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroups
{
    public sealed class GetStudentGroupsResponse
    {
        public List<GetStudentGroupItem> Groups { get; init; } = [];
    }

    public sealed class GetStudentGroupItem
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public int Grade { get; init; }
        public string Teacher { get; init; } = string.Empty;
        public GetStudentGroupStats Stats { get; init; } = new();
        public double ProgressPercentage => Stats.Lessons.Total == 0
            ? 0
            : (double)Stats.Lessons.Completed / Stats.Lessons.Total * 100;
    }

    public sealed class GetStudentGroupStats
    {
        public int StudentsCount { get; init; }
        public GetStudentGroupLessons Lessons { get; init; } = new();
    }

    public sealed class GetStudentGroupLessons
    {
        public int Completed { get; init; }
        public int Total { get; init; }
    }
}
