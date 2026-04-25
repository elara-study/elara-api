namespace Elara.Infrastructure.Chat
{
    public class GeminiSettings
    {
        public const string SectionName = "Gemini";
        public string ApiKey { get; set; } = string.Empty;
        public int ContextWindowSize { get; set; } = 5;
    }
}
