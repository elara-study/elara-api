namespace Elara.Application.Features.ChatAnalysisReport
{
    public record ChatAnalysisJob(
        Guid ConversationId,
        Guid StudentId,
        string Subject
    );
}
