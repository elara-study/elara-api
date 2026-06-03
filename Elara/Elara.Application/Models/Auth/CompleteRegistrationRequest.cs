namespace Elara.Application.Models.Auth
{
    public class CompleteRegistrationRequest
    {
        public string PendingToken { get; set; } = string.Empty;
        public string Role         { get; set; } = string.Empty;
        public int?   SubjectId    { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int?   Grade        { get; set; }
    }
}
