using Elara.Domain.Entities.Users;

namespace Elara.Domain.Entities.Chat
{
    public class Conversation : BaseEntity<Guid>
    {
        public Guid StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Student? Student { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; } = [];
    }
}
