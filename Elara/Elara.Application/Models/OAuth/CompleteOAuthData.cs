namespace Elara.Application.Models.OAuth
{
    public class CompleteOAuthData
    {
        public string Provider       { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
        public string Name           { get; set; } = string.Empty;
        public string Role           { get; set; } = string.Empty;
        public int?   SubjectId      { get; set; }
        public DateTime DateOfBirth  { get; set; }
    }
}
