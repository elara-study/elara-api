using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Chat
{
    public class ChatAnalysisReport : BaseEntity
    {
        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        public Guid ConversationId { get; set; }
        public Guid StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ReportText { get; set; } = string.Empty;
        public int AnalyzedMessageCount { get; set; }

        public virtual Conversation? Conversation { get; set; }
        public virtual Users.Student? Student { get; set; }
    }
}
