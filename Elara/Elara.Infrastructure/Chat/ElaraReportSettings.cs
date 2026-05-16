namespace Elara.Infrastructure.Chat
{
    public class ElaraReportSettings
    {
        public const string SectionName = "ElaraReport";
        public string EndpointUrl { get; set; } = string.Empty;
        public int MinMessageThreshold { get; set; } = 6;
        public int RunIntervalMinutes { get; set; } = 60;
        public int QueueCapacity { get; set; } = 500;
    }
}
