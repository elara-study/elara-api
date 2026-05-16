namespace Elara.Application.Common.Interfaces
{
    public interface IElaraReportService
    {
        Task<string> AnalyzeConversationAsync(
            string transcript,
            CancellationToken cancellationToken = default);
    }
}
