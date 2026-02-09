namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string JoinCode { get; set; } = string.Empty;
    }
}