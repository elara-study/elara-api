namespace Elara.Application.Features.Quiz.DTOs
{
    public class HintDto
    {
        public int HintId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int HintLevel { get; set; }
    }
}
