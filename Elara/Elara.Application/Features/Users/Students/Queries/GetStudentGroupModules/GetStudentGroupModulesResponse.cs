namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupModules
{
    public class GetStudentGroupModulesResponse
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public List<ModuleDto> Modules { get; set; } = new();
    }

    public class ModuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
