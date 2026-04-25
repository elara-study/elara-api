namespace Elara.Application.Features.Chat
{
    public class ChatSettings
    {
        public const string SectionName = "Gemini";
        public int ContextWindowSize { get; set; } = 5;
    }
}
